
using RTNet.ImgCore;

namespace RTNet
{
  public static class Program
  {
    public static void Main(string[] args)
    {
      try
      {
        var application = new Application(new ApplicationSpec { Name = "RT.NET", Width = 1700, Height = 1200 });
        application.AddAppLayer(new RayTracerAppLayer());
        application.Run();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
    }
  }
}