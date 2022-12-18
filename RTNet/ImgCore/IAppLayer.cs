using Veldrid;

namespace RTNet.ImgCore
{
  public interface IAppLayer
  {
    void OnAttach();
    void OnDetach();

    void OnUpdate(float ts);
    void OnUIRender();
  }
}