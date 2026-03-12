namespace IperfApp.UI;

public partial class Form1
{
  // Logique des tests
  private async Task RunFullTest()
  {
    if (string.IsNullOrWhiteSpace(txtServer.Text))
    {
      MessageBox.Show("Veuillez entrer l'adresse du serveur.", "Champ requis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      return;
    }

    btnStart.Enabled = false;
    btnStart.Text = "ANALYSE EN COURS...";
    btnStart.BackColor = Color.FromArgb(160, 174, 192);
    txtLog.Clear();

    txtLog.AppendText(" [SYSTÈME] Démarrage des flux..." + Environment.NewLine);
    txtLog.AppendText(" >>> FLUX MONTANT (UPLOAD)" + Environment.NewLine);
    _lastUp = await _engine.ExecuteAsync(txtServer.Text, txtPort.Text, txtChannels.Text, false);

    txtLog.AppendText(Environment.NewLine + " <<< FLUX DESCENDANT (DOWNLOAD)" + Environment.NewLine);
    _lastDown = await _engine.ExecuteAsync(txtServer.Text, txtPort.Text, txtChannels.Text, true);

    txtLog.AppendText(Environment.NewLine + " ╔══════════════════════════════════════╗" + Environment.NewLine);
    txtLog.AppendText($" ║  RÉSULTATS DE LA MESURE              ║" + Environment.NewLine);
    txtLog.AppendText($" ║  Upload   : {_lastUp,10:F2} Mbps          ║" + Environment.NewLine);
    txtLog.AppendText($" ║  Download : {_lastDown,10:F2} Mbps          ║" + Environment.NewLine);
    txtLog.AppendText(" ╚══════════════════════════════════════╝" + Environment.NewLine);

    btnStart.Enabled = true;
    btnStart.Text = "RELANCER L'ANALYSE";
    btnStart.BackColor = _colorAccent;
    btnExportNew.Enabled = btnExportAppend.Enabled = true;
    btnExportNew.FlatAppearance.BorderColor = btnExportAppend.FlatAppearance.BorderColor = _colorAccent;
  }
}