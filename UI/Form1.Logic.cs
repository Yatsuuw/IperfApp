using IperfApp.Services;

namespace IperfApp.UI;

public partial class Form1 : Form
{
  private async Task RunFullTest()
  {
    // Validation rapide
    if (string.IsNullOrWhiteSpace(txtServer.Text))
    {
      MessageBox.Show("Veuillez entrer l'adresse du serveur.", "Champ requis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      return;
    }

    // Préparation UI
    btnStart.Enabled = false;
    btnStart.Text = "ANALYSE EN COURS...";
    btnStart.BackColor = Color.FromArgb(160, 174, 192);
    txtLog.Clear();

    // Récupération des paramètres
    string server = txtServer.Text;
    string port = string.IsNullOrWhiteSpace(txtPort.Text) ? "5201" : txtPort.Text;
    string channels = string.IsNullOrWhiteSpace(txtChannels.Text) ? "1" : txtChannels.Text;

    // Exécution des tests via le moteur Iperf
    txtLog.AppendText(" [SYSTÈME] Initialisation des flux..." + Environment.NewLine);
    txtLog.AppendText(" >>> FLUX MONTANT (UPLOAD)" + Environment.NewLine);
    _lastUp = await _engine.ExecuteAsync(server, port, channels, false);

    txtLog.AppendText(Environment.NewLine + " <<< FLUX DESCENDANT (DOWNLOAD)" + Environment.NewLine);
    _lastDown = await _engine.ExecuteAsync(server, port, channels, true);

    // Affichage du résumé final stylisé
    txtLog.AppendText(Environment.NewLine + " ╔══════════════════════════════════════╗" + Environment.NewLine);
    txtLog.AppendText(" ║  RÉSULTATS DE LA MESURE              ║" + Environment.NewLine);
    txtLog.AppendText($" ║  Upload   : {_lastUp,10:F2} Mbps          ║" + Environment.NewLine);
    txtLog.AppendText($" ║  Download : {_lastDown,10:F2} Mbps          ║" + Environment.NewLine);
    txtLog.AppendText(" ╚══════════════════════════════════════╝" + Environment.NewLine);

    // Réactivation de l'interface et feedback visuel
    btnStart.Enabled = true;
    btnStart.Text = "RELANCER L'ANALYSE";
    btnStart.BackColor = _colorAccent;

    btnExportNew.Enabled = true;
    btnExportAppend.Enabled = true;

    // Activation des bordures pour indiquer que l'export est prêt
    btnExportNew.FlatAppearance.BorderColor = _colorAccent;
    btnExportAppend.FlatAppearance.BorderColor = _colorAccent;
  }

  // Gestion de l'exportation
  private void HandleSave(bool append)
  {
    // Choix du fichier
    using FileDialog fd = append ? new OpenFileDialog() : new SaveFileDialog();
    fd.Filter = "Fichier CSV|*.csv";
    fd.Title = append ? "Sélectionner un fichier CSV existant" : "Créer un nouveau rapport CSV";

    if (fd.ShowDialog() == DialogResult.OK)
    {
      try 
      {
        // Appel au service d'exportation
        CsvExporter.Save(fd.FileName, _lastUp, _lastDown, append, txtServer.Text, txtPort.Text, txtChannels.Text);
        
        MessageBox.Show("Le rapport a été enregistré avec succès !", 
          "Export terminé", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Erreur lors de l'enregistrement : {ex.Message}", 
          "Erreur d'export", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
  }
}