
using RTNet.ImgCore;

namespace RTNet
{
  public static class Program
  {
    public static void Main(string[] args)
    {
      var spec = new ApplicationSpec { Name = "Foo" };

      try
      {

        using (var application = new Application(spec))
        {
          application.AddAppLayer(new RayTracerAppLayer());
          application.Run();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
    }
  }
}