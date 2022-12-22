
using RTNet.ImgCore;
using RTNet.Maze;

namespace RTNet
{
  public static class Program
  {
    public static void Main(string[] args)
    {
      try
      {
        var application = new Application(new ApplicationSpec { Name = "RT.NET", Width = 1700, Height = 1200 });
        var appInfo = new AppInfo();
        application.AddAppLayer(new RayTracerAppLayer(appInfo));
        // application.AddAppLayer(new MazeAppLayer(appInfo));
        application.Run(appInfo);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
    }
  }
}