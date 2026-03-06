namespace IperfApp.Services;

public static class CsvExporter
{
  public static void Save(string path, double up, double down, bool append, string server, string port, string channels)
  {
    string h = "Date;Heure;Serveur;Port;Canaux;Upload_Mbps;Download_Mbps";
    string l = $"{DateTime.Now:dd/MM/yyyy};{DateTime.Now:HH:mm:ss};{server};{port};{channels};{up:F2};{down:F2}";
    using StreamWriter sw = new(path, append);
    if (!append || !File.Exists(path) || new FileInfo(path).Length == 0) sw.WriteLine(h);
    sw.WriteLine(l);
  }

  internal static void Save(string fileName, object lastUp, object lastDown, bool append)
  {
    throw new NotImplementedException();
  }
}