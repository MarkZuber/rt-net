using Veldrid;

namespace RTNet.ImgCore
{
  public interface IAppLayer
  {
    void OnUpdate(float ts);
    void OnUIRender();
  }
}