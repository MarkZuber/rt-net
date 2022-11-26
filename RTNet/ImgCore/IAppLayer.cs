using Veldrid;

namespace RTNet.ImgCore
{
  public interface IAppLayer
  {
    void Initialize(ImGuiController controller);

    void OnAttach();
    void OnDetach();

    void OnUpdate(float ts);
    void OnUIRender();
  }
}