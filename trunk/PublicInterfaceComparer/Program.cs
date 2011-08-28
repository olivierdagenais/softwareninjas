using System.IO;
using SoftwareNinjas.NAnt.Tasks;

namespace SoftwareNinjas.PublicInterfaceComparer
{
    class Program
    {
        static void Main(string[] args)
        {
            var pic = new PublicInterfaceComparerTask(false)
            {
                BaselineFile = new FileInfo(args[0]),
                ChallengerFile = new FileInfo(args[1]),
                ReportFile = new FileInfo(args[2]),
            };
            pic.ExecuteForTest();
        }
    }
}
