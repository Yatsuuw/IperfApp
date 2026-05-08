// Usings globaux — seuls les namespaces NON couverts par <ImplicitUsings>enable</ImplicitUsings> sont listés ici.
// System, System.Collections.Generic, System.Linq,
// System.Threading et System.Threading.Tasks sont déjà injectés automatiquement.
// System.Text est conservé explicitement car ImplicitUsings ne l'injecte PAS
// pour les projets de type WinForms (contrairement aux projets Console/Web).
// System.Globalization est centralisé ici pour éviter les régressions CS0103
// sur CultureInfo (utilisé dans CsvExporter et IperfEngine).
global using System.Diagnostics;
global using System.Globalization;
global using System.IO;
global using System.Text;
global using System.Windows.Forms;
global using System.Drawing;
