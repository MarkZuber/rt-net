using System.Numerics;
using ImGuiNET;

namespace RTNet
{
  public class Camera
  {

    private float _verticalFOV = 45.0f;
    private float _nearClip = 0.1f;
    private float _farClip = 100.0f;

    private Vector3 _forwardDirection = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 _position = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector2 _lastMousePosition = new Vector2(0.0f, 0.0f);

    private Matrix4x4 _projection = new Matrix4x4(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
    private Matrix4x4 _view = new Matrix4x4(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
    private Matrix4x4 _inverseProjection = new Matrix4x4(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
    private Matrix4x4 _inverseView = new Matrix4x4(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);

    private Vector3[] _rayDirections = new Vector3[1];
    private UInt32 _viewportWidth = 0;
    private UInt32 _viewportHeight = 0;


    public Camera(float verticalFOV, float nearClip, float farClip)
    {
      _verticalFOV = verticalFOV;
      _nearClip = nearClip;
      _farClip = farClip;

      _forwardDirection = new Vector3(0, 0, -1);
      _position = new Vector3(0, 0, 6);
    }

    public Vector3 Position => _position;
    public Vector3[] RayDirections => _rayDirections;

    public void OnUpdate(float ts)
    {
      var io = ImGui.GetIO();
      var mousePos = io.MousePos;
      var delta = (mousePos - _lastMousePosition) * 0.002f;
      _lastMousePosition = mousePos;

      // right mouse button
      if (!io.MouseDown[1])
      {
        // 
        return;
      }

      var moved = false;

      var upDirection = new Vector3(0.0f, 1.0f, 0.0f);
      var rightDirection = Vector3.Cross(_forwardDirection, upDirection);

      float speed = 5.0f;

      if (io.KeysDown[(int)Veldrid.Key.W])
      {
        _position += _forwardDirection * speed * ts;
        moved = true;
      }
      else if (io.KeysDown[(int)Veldrid.Key.S])
      {
        _position -= _forwardDirection * speed * ts;
        moved = true;
      }
      else if (io.KeysDown[(int)Veldrid.Key.A])
      {
        _position -= rightDirection * speed * ts;
        moved = true;
      }
      else if (io.KeysDown[(int)Veldrid.Key.D])
      {
        _position += rightDirection * speed * ts;
        moved = true;
      }
      else if (io.KeysDown[(int)Veldrid.Key.Q])
      {
        _position -= upDirection * speed * ts;
        moved = true;
      }
      else if (io.KeysDown[(int)Veldrid.Key.E])
      {
        _position += upDirection * speed * ts;
        moved = true;
      }

      // Rotation
      if (delta.X != 0.0f || delta.Y != 0.0f)
      {
        var pitchDelta = delta.Y * GetRotationSpeed();
        var yawDelta = delta.X * GetRotationSpeed();

        var foo = Quaternion.CreateFromAxisAngle(rightDirection, -pitchDelta);

        var q = Quaternion.CreateFromYawPitchRoll(-yawDelta, -pitchDelta, 0.0f);
        // var q = Quaternion.Normalize(Quaternion.Cross(Quaternion.CreateFromAxisAngle(rightDirection, -pitchDelta), Quaternion.CreateFromAxisAngle(new Vector3(0.0f, 1.0f, 0.0f), -yawDelta)));
        _forwardDirection = Vector3.Transform(_forwardDirection, q);
        moved = true;
      }

      if (moved)
      {
        RecalculateView();
        RecalculateRayDirections();
      }
    }

    public void OnResize(UInt32 width, UInt32 height)
    {
      if (width == _viewportWidth && height == _viewportHeight)
      {
        return;
      }

      _viewportWidth = width;
      _viewportHeight = height;
      RecalculateView();
      RecalculateProjection();
      RecalculateRayDirections();
    }

    private float GetRotationSpeed()
    {
      return 0.3f;
    }

    private void RecalculateProjection()
    {
      _projection = Matrix4x4.CreatePerspectiveFieldOfView(
        (_verticalFOV * Convert.ToSingle(Math.PI)) / 180.0f,
        (float)_viewportWidth / (float)_viewportHeight,
        _nearClip,
        _farClip);
      Matrix4x4.Invert(_projection, out _inverseProjection);
    }

    private void RecalculateView()
    {
      _view = Matrix4x4.CreateLookAt(_position, _position + _forwardDirection, new Vector3(0, 1, 0));
      Matrix4x4.Invert(_view, out _inverseView);
    }

    private void RecalculateRayDirections()
    {
      _rayDirections = new Vector3[(int)(_viewportWidth * _viewportHeight)];

      for (UInt32 y = 0; y < _viewportHeight; y++)
      {
        for (UInt32 x = 0; x < _viewportWidth; x++)
        {
          Vector2 coord = new Vector2((float)x / (float)_viewportWidth, (float)y / (float)_viewportHeight);

          // -1 -> 1
          coord = coord * 2.0f;
          coord = new Vector2(coord.X - 1.0f, coord.Y - 1.0f);

          var target = Vector4.Transform(new Vector4(coord.X, coord.Y, 1.0f, 1.0f), _inverseProjection);

          var partialWorldSpace = Vector4.Transform(new Vector4(Vector3.Normalize(new Vector3(target.X, target.Y, target.Z) / target.W), 0.0f), _inverseView);
          var rayDirection = new Vector3(partialWorldSpace.X, partialWorldSpace.Y, partialWorldSpace.Z);  // world space
          _rayDirections[(int)(x + y * _viewportWidth)] = rayDirection;
        }
      }
    }
  }
}