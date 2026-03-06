using IperfApp.Services;

namespace IperfApp.UI;

public partial class Form1 : Form
{
  // Lance le test complet
  private async Task RunFullTest()
  {
    // Validation champs
    if (string.IsNullOrWhiteSpace(txtServer.Text))
    {
      MessageBox.Show("Veuillez entrer l'adresse du serveur", "Champ requis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      return;
    }

    // Mise à jour interface
    btnStart.Enabled = false;
    btnStart.Text = "ANALYSE EN COURS...";
    btnStart.BackColor = Color.FromArgb(160, 174, 192);
    txtLog.Clear();

    // Récupération valeurs
    string server = txtServer.Text;
    string port = string.IsNullOrWhiteSpace(txtPort.Text) ? "5201" : txtPort.Text;
    string channels = string.IsNullOrWhiteSpace(txtChannels.Text) ? "1" : txtChannels.Text;

    // Test upload
    txtLog.AppendText(" [SYSTÈME] Démarrage des tests..." + Environment.NewLine);
    txtLog.AppendText(" >>> FLUX MONTANT (UPLOAD)" + Environment.NewLine);
    _lastUp = await _engine.ExecuteAsync(server, port, channels, false);

    // Test download
    txtLog.AppendText(Environment.NewLine + " <<< FLUX DESCENDANT (DOWNLOAD)" + Environment.NewLine);
    _lastDown = await _engine.ExecuteAsync(server, port, channels, true);

    // Affichage résumé
    txtLog.AppendText(Environment.NewLine + " ╔════════════════════════════════════════════╗" + Environment.NewLine);
    txtLog.AppendText(" ║  SYNTHÈSE DES RÉSULTATS                    ║" + Environment.NewLine);
    txtLog.AppendText($" ║  Upload   : {_lastUp,10:F2} Mbps                ║" + Environment.NewLine);
    txtLog.AppendText($" ║  Download : {_lastDown,10:F2} Mbps                ║" + Environment.NewLine);
    txtLog.AppendText(" ╚════════════════════════════════════════════╝" + Environment.NewLine);

    // Mise à jour interface
    btnStart.Enabled = true;
    btnStart.Text = "RELANCER L'ANALYSE";
    btnStart.BackColor = _colorAccent;
    btnExportNew.Enabled = btnExportAppend.Enabled = true;
  }

  // Export CSV
  private void HandleSave(bool append)
  {
    // Explorateur Windows
    FileDialog fd = append ? new OpenFileDialog() : new SaveFileDialog();
    fd.Filter = "Fichier CSV|*.csv";

    if (fd.ShowDialog() == DialogResult.OK)
    {
      // Sauvegarde
      CsvExporter.Save(fd.FileName, _lastUp, _lastDown, append, txtServer.Text, txtPort.Text, txtChannels.Text);

      // Confirmation
      MessageBox.Show("Rapport enregistré avec succès !", "Exportation réussie", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
  }
}
