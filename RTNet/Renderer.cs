using RTNet.ImgCore;
using Veldrid;
using System.Numerics;

namespace RTNet
{

  public struct Ray
  {
    public Vector3 Origin { get; set; }
    public Vector3 Direction { get; set; }
  }

  struct HitPayload
  {
    public float HitDistance { get; set; }
    public Vector3 WorldPosition { get; set; }
    public Vector3 WorldNormal { get; set; }

    public int ObjectIndex { get; set; }
  };

  public struct Material
  {
    public Material() { }

    public Vector3 Albedo { get; set; } = new Vector3(1.0f);
    public float Roughness { get; set; } = 1.0f;
    public float Metallic { get; set; } = 0.0f;
  };

  public struct Sphere
  {
    public Sphere() { }
    public Vector3 Position { get; set; } = new Vector3(0.0f);
    public float Radius { get; set; } = 0.5f;

    public int MaterialIndex { get; set; } = 0;
  };

  public struct Scene
  {
    public Scene() { }
    public List<Sphere> Spheres { get; } = new List<Sphere>();
    public List<Material> Materials = new List<Material>();
  };

  public class Renderer
  {
    private Random _random = new Random();

    private ImageBuffer _finalImageBuffer;
    private Camera _camera;
    private Scene _scene;

    private Vector3 RandomVector(float min, float max)
    {
      return new Vector3(
        _random.NextSingle() * (max - min) + min,
        _random.NextSingle() * (max - min) + min,
        _random.NextSingle() * (max - min) + min);
    }

    public Renderer(IAppInfo appInfo, UInt32 width, UInt32 height)
    {
      _finalImageBuffer = new ImageBuffer(appInfo, width, height);
      _camera = new Camera(0.0f, 0.0f, 0.0f);
    }

    public UInt32 Width => _finalImageBuffer.Width;
    public UInt32 Height => _finalImageBuffer.Height;

    public IntPtr GetFinalImagePtr()
    {
      return _finalImageBuffer.CaptureImageBufferPointer();
    }

    public void Resize(UInt32 width, UInt32 height)
    {
      _finalImageBuffer.Resize(width, height);
    }

    private Vector4 PerPixelRandom(UInt32 x, UInt32 y)
    {
      return new Vector4(Convert.ToSingle(_random.NextDouble()), Convert.ToSingle(_random.NextDouble()), Convert.ToSingle(_random.NextDouble()), 0.0f);
    }

    private Vector4 PerPixel(UInt32 x, UInt32 y)
    {
      Ray ray = new Ray();
      ray.Origin = _camera.Position;
      ray.Direction = _camera.RayDirections[(int)(x + y * Width)];

      Vector3 color = new Vector3(0.0f);
      float multiplier = 1.0f;

      int bounces = 5;
      for (int i = 0; i < bounces; i++)
      {
        HitPayload payload = TraceRay(ray);
        if (payload.HitDistance < 0.0f)
        {
          var skyColor = new Vector3(0.6f, 0.7f, 0.9f);
          color += skyColor * multiplier;
          break;
        }

        var lightDir = Vector3.Normalize(new Vector3(-1.0f, -1.0f, -1.0f));
        float lightIntensity = Math.Max(Vector3.Dot(payload.WorldNormal, -lightDir), 0.0f); // == cos(angle)

        var sphere = _scene.Spheres[payload.ObjectIndex];
        var material = _scene.Materials[sphere.MaterialIndex];

        Vector3 sphereColor = material.Albedo;
        sphereColor *= lightIntensity;
        color += sphereColor * multiplier;

        multiplier *= 0.5f;

        ray.Origin = payload.WorldPosition + payload.WorldNormal * 0.0001f;
        ray.Direction = Vector3.Reflect(ray.Direction,
            payload.WorldNormal + material.Roughness * RandomVector(-0.5f, 0.5f));
      }

      return Vector4.Clamp(new Vector4(color, 1.0f), new Vector4(0.0f), new Vector4(1.0f));
    }
    private HitPayload TraceRay(Ray ray)
    {
      // (bx^2 + by^2)t^2 + (2(axbx + ayby))t + (ax^2 + ay^2 - r^2) = 0
      // where
      // a = ray origin
      // b = ray direction
      // r = radius
      // t = hit distance

      int closestSphere = -1;
      float hitDistance = Single.MaxValue;
      for (int i = 0; i < _scene.Spheres.Count(); i++)
      {
        var sphere = _scene.Spheres[i];
        Vector3 origin = ray.Origin - sphere.Position;

        float a = Vector3.Dot(ray.Direction, ray.Direction);
        float b = 2.0f * Vector3.Dot(origin, ray.Direction);
        float c = Vector3.Dot(origin, origin) - sphere.Radius * sphere.Radius;

        // Quadratic forumula discriminant:
        // b^2 - 4ac

        float discriminant = b * b - 4.0f * a * c;
        if (discriminant < 0.0f)
          continue;

        // Quadratic formula:
        // (-b +- sqrt(discriminant)) / 2a

        // float t0 = (-b + glm::sqrt(discriminant)) / (2.0f * a); // Second hit distance (currently unused)
        float closestT = (-b - Convert.ToSingle(Math.Sqrt(discriminant))) / (2.0f * a);
        if (closestT > 0.0f && closestT < hitDistance)
        {
          hitDistance = closestT;
          closestSphere = (int)i;
        }
      }

      if (closestSphere < 0)
        return Miss(ray);

      return ClosestHit(ray, hitDistance, closestSphere);
    }

    private HitPayload ClosestHit(Ray ray, float hitDistance, int objectIndex)
    {

      HitPayload payload = new HitPayload();
      payload.HitDistance = hitDistance;
      payload.ObjectIndex = objectIndex;

      var closestSphere = _scene.Spheres[objectIndex];

      Vector3 origin = ray.Origin - closestSphere.Position;
      payload.WorldPosition = origin + ray.Direction * hitDistance;
      payload.WorldNormal = Vector3.Normalize(payload.WorldPosition);

      payload.WorldPosition += closestSphere.Position;

      return payload;
    }

    private HitPayload Miss(Ray ray)
    {
      HitPayload payload = new HitPayload();
      payload.HitDistance = -1.0f;
      return payload;
    }

    public void Render(Camera camera, Scene scene)
    {
      _camera = camera;
      _scene = scene;
      for (UInt32 y = 0; y < Height; y++)
      {
        for (UInt32 x = 0; x < Width; x++)
        {
          _finalImageBuffer.SetPixel(x, y, PerPixel(x, y));
        }
      }
    }
  }
}