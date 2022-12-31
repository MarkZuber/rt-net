// -----------------------------------------------------------------------
// <copyright file="NffParser.cs" company="ZubeNET">
//   Copyright...
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using WkndRay.Hitables;
using WkndRay.Materials;
using WkndRay.Textures;

namespace WkndRay.Scenes
{
  public class NffParser
  {
    public static IScene ParseFile(string filePath, int imageWidth, int imageHeight)
    {
      var background = new Func<Ray, Vector4>(ray => new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
      var lights = new HitableList();
      var world = new HitableList();
      var cameraAt = new Vector3(0.0f, 0.0f, 0.0f);
      var cameraFrom = new Vector3(0.0f, 0.0f, 0.0f);
      var cameraUp = new Vector3(0.0f, 0.0f, 0.0f);

      var lookingFor = LookingFor.Instruction;

      IMaterial currentMaterial = new LambertianMaterial(new ColorTexture(0.0f, 0.0f, 0.0f));

      var polyVectors = new List<Vector3>();
      var currentItemCounter = 0;

      var lines = File.ReadAllLines(filePath);

      foreach (var line in lines)
      {
        var split = line.Split(' ', '\t');

        switch (lookingFor)
        {
          case LookingFor.Instruction:
            {
              var instruction = split[0];

              if (instruction == "b")
              {
                // background color
                background = ray => new Vector4(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]), 1.0f);
              }
              else if (instruction == "v")
              {
                // viewpoint location
                lookingFor = LookingFor.ViewpointFrom;
              }
              else if (instruction == "l")
              {
                // positional light
                //var Vector4 = split.Length == 7
                //                    ? new Vector4(
                //                      float.Parse(split[4]),
                //                      float.Parse(split[5]),
                //                      float.Parse(split[6]))
                //                    : new Vector4(7.0f, 7.0f, 7.0f);
                var Vector4 = new Vector4(7.0f, 7.0f, 7.0f, 1.0f);
                var sphere = new Sphere(
                  new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3])),
                  1.5f,
                  new DiffuseLight(new ColorTexture(Vector4)));
                lights.Add(sphere);
                world.Add(sphere);
              }
              else if (instruction == "f")
              {
                // println!("reading f: {}", num);
                // object material properties
                // "f" red green blue Kd Ks Shine T index_of_refraction
                // Kd Diffuse component
                // Ks Specular
                // Shine Phong cosine power for highlights
                // T Transmittance (fraction of contribution of the transmitting ray).
                // Usually, 0 <= Kd <= 1 and 0 <= Ks <= 1, though it is not required that Kd + Ks = 1. Note that transmitting objects (T > 0) are considered to have two sides for algorithms that need these (normally, objects have one side).
                // todo: i don't think i'm assigning the correct values into my solidmaterial yet
                //currentMaterial = new SolidMaterial(
                //  0.0f, // kAmbient
                //  float.Parse(split[4]),  // kDiffuse
                //  float.Parse(split[5]),  // kSpecular
                //  float.Parse(split[7]),  // kReflection
                //  float.Parse(split[8]),  // kTransparent
                //  float.Parse(split[8]),  // refraction    -- todo: which is which here?
                //  float.Parse(split[6]),  // gloss
                //  new Vector4(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]))
                //);
                // todo: need to look at diffuse/specular/reflection/transparent and pick an appropriate material for it...
                //currentMaterial = new LambertianMaterial(
                //  new ColorTexture(
                //    new Vector4(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]))));
                if (float.Parse(split[5]) > 0.01)
                {
                  currentMaterial = new MetalMaterial(
                    new Vector4(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]), 1.0f),
                    1.5f);
                }
                else
                {
                  currentMaterial = new LambertianMaterial(
                    new ColorTexture(
                      new Vector4(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]), 1.0f)));
                }
              }
              else if (instruction == "c")
              {
                // cone or cylinder
              }
              else if (instruction == "s")
              {
                // sphere
                world.Add(
                  new Sphere(
                    new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3])),
                    float.Parse(split[4]),
                    currentMaterial));
              }
              else if (instruction == "p")
              {
                // polygon
                currentItemCounter = int.Parse(split[1]);
                polyVectors = new List<Vector3>();
                lookingFor = LookingFor.Polygon;
              }
              else if (instruction == "pp")
              {
                // polygon patch
              }
              else if (instruction == "#")
              {
                // comment
              }
            }
            break;
          case LookingFor.Polygon:
            {
              if (currentItemCounter > 0)
              {
                currentItemCounter--;
                polyVectors.Add(new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2])));
              }

              if (currentItemCounter == 0)
              {
                if (polyVectors.Count >= 3)
                {
                  var firstVert = polyVectors[0];
                  var prevVert = polyVectors[1];
                  var thisVert = polyVectors[2];
                  world.Add(
                    new Triangle(
                      new List<Vector3>
                      {
                    firstVert,
                    prevVert,
                    thisVert
                      },
                      currentMaterial));

                  for (var i = 3; i < polyVectors.Count; i++)
                  {
                    prevVert = thisVert;
                    thisVert = polyVectors[i];
                    world.Add(
                      new Triangle(
                        new List<Vector3>
                        {
                      firstVert,
                      prevVert,
                      thisVert
                        },
                        currentMaterial));
                  }
                }

                lookingFor = LookingFor.Instruction;
              }
            }
            break;
          case LookingFor.ViewpointFrom:
            {
              cameraFrom = new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]));
              lookingFor = LookingFor.ViewpointAt;
            }
            break;
          case LookingFor.ViewpointAt:
            {
              cameraAt = new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]));
              lookingFor = LookingFor.ViewpointUp;
            }
            break;
          case LookingFor.ViewpointUp:
            {
              cameraUp = new Vector3(float.Parse(split[1]), float.Parse(split[2]), float.Parse(split[3]));
              lookingFor = LookingFor.ViewpointAngle;
            }
            break;
          case LookingFor.ViewpointAngle:
            {
              // todo: implement
              lookingFor = LookingFor.ViewpointHither;
            }
            break;
          case LookingFor.ViewpointHither:
            {
              // todo: implement
              lookingFor = LookingFor.ViewpointResolution;
            }
            break;
          case LookingFor.ViewpointResolution:
            {
              //resolutionX = int.Parse(split[1]);
              //resolutionY = int.Parse(split[2]);

              lookingFor = LookingFor.Instruction;
            }
            break;
        }
      }

      return new NffParserScene(
        new Camera(
          cameraFrom,
          cameraAt,
          cameraUp,
          50.0f,
          Convert.ToSingle(imageWidth) / Convert.ToSingle(imageHeight),
          0.0f,
          10.0f),
        new BvhNode(world, 0.0f, 1.0f),
        lights,
        background);
    }

    private enum LookingFor
    {
      Instruction,
      ViewpointFrom,
      ViewpointAt,
      ViewpointUp,
      ViewpointAngle,
      ViewpointHither,
      ViewpointResolution,
      Polygon
    }

    private class NffParserScene : IScene
    {
      private readonly Func<Ray, Vector4> _backgroundFunc;

      private readonly Camera _camera;
      private readonly IHitable _lightHitable;
      private readonly IHitable _world;

      public NffParserScene(Camera camera, IHitable world, IHitable lightHitable, Func<Ray, Vector4> backgroundFunc)
      {
        _camera = camera;
        _world = world;
        _lightHitable = lightHitable;
        _backgroundFunc = backgroundFunc;
      }

      public Camera GetCamera(uint imageWidth, uint imageHeight)
      {
        return _camera;
      }

      public IHitable GetWorld()
      {
        return _world;
      }

      public IHitable GetLightHitable()
      {
        return _lightHitable;
      }

      public Func<Ray, Vector4> GetBackgroundFunc()
      {
        return _backgroundFunc;
      }
    }
  }
}
