using System.Windows.Forms.DataVisualization.Charting;
using IperfApp.Models;

namespace IperfApp.UI;

public partial class Form1
{
  // ---------------------------------------------------------------
  // Builder : section Historique (ListView + Chart)
  // ---------------------------------------------------------------

  /// <summary>
  /// Construit et positionne le panel d'historique sous la zone d'actions.
  /// Initialise <see cref="_lvHistory"/> et <see cref="_chart"/>.
  /// </summary>
  private void BuildHistoryArea()
  {
    // Calculer Y de départ : après les boutons export
    // export = CardBottom + 15 (btnStart H50) + 5 (btnCancel H30) + 15 (log H170) + 5 + 45 (export)
    // On fixe directement la position relative à CardBottom pour robustesse
    const int spacer   = 15;
    const int btnMainH = 50;
    const int btnCancelH = 30;
    const int logH     = 170;
    const int exportH  = 45;
    int topHistory = CardBottom + spacer + btnMainH + (spacer / 3)
                   + btnCancelH + spacer + logH + (spacer / 3)
                   + exportH + spacer;

    int left  = CardLeft;
    int width = CardWidth;

    // --- Titre section ---
    var lblHistory = new Label
    {
      Text      = "HISTORIQUE DES MESURES",
      Font      = new Font("Segoe UI Semibold", 9F),
      ForeColor = _colorAccent,
      Location  = new Point(left, topHistory),
      Size      = new Size(width, 20),
      TextAlign = ContentAlignment.MiddleLeft
    };
    Controls.Add(lblHistory);

    topHistory += 25;

    // --- Graphique ---
    _chart = new Chart
    {
      Location    = new Point(left, topHistory),
      Size        = new Size(width, 160),
      BackColor   = _colorCard,
      BorderlineColor = Color.FromArgb(230, 235, 240),
      BorderlineDashStyle = ChartDashStyle.Solid,
      BorderlineWidth = 1
    };

    var chartArea = new ChartArea("main")
    {
      BackColor       = _colorCard,
      BorderColor     = Color.FromArgb(230, 235, 240),
      BorderDashStyle = ChartDashStyle.Solid
    };
    chartArea.AxisX.LabelStyle.Font    = new Font("Segoe UI", 7F);
    chartArea.AxisX.LabelStyle.ForeColor = Color.DimGray;
    chartArea.AxisX.LineColor          = Color.FromArgb(220, 225, 230);
    chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(240, 242, 245);
    chartArea.AxisX.Title              = "Mesure";
    chartArea.AxisX.TitleFont          = new Font("Segoe UI", 7F);
    chartArea.AxisX.TitleForeColor     = Color.DimGray;
    chartArea.AxisY.LabelStyle.Font    = new Font("Segoe UI", 7F);
    chartArea.AxisY.LabelStyle.ForeColor = Color.DimGray;
    chartArea.AxisY.LineColor          = Color.FromArgb(220, 225, 230);
    chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(240, 242, 245);
    chartArea.AxisY.Title              = "Mbps";
    chartArea.AxisY.TitleFont          = new Font("Segoe UI", 7F);
    chartArea.AxisY.TitleForeColor     = Color.DimGray;
    _chart.ChartAreas.Add(chartArea);

    var legend = new Legend
    {
      Font      = new Font("Segoe UI", 7.5F),
      BackColor = _colorCard,
      Docking   = Docking.Bottom
    };
    _chart.Legends.Add(legend);

    var seriesUp = new Series("Upload")
    {
      ChartType  = SeriesChartType.Line,
      Color      = Color.FromArgb(0, 120, 215),
      BorderWidth = 2,
      MarkerStyle = MarkerStyle.Circle,
      MarkerSize  = 6,
      ChartArea   = "main"
    };
    var seriesDown = new Series("Download")
    {
      ChartType  = SeriesChartType.Line,
      Color      = Color.FromArgb(16, 163, 127),
      BorderWidth = 2,
      MarkerStyle = MarkerStyle.Circle,
      MarkerSize  = 6,
      ChartArea   = "main"
    };
    _chart.Series.Add(seriesUp);
    _chart.Series.Add(seriesDown);

    topHistory += 165;

    // --- ListView ---
    _lvHistory = new ListView
    {
      Location      = new Point(left, topHistory),
      Size          = new Size(width, 140),
      View          = View.Details,
      FullRowSelect = true,
      GridLines     = false,
      Font          = new Font("Segoe UI", 8.5F),
      BackColor     = _colorCard,
      ForeColor     = Color.FromArgb(50, 50, 50),
      BorderStyle   = BorderStyle.FixedSingle,
      HeaderStyle   = ColumnHeaderStyle.Nonclickable
    };
    _lvHistory.Columns.AddRange([
      new ColumnHeader { Text = "Heure",          Width = 70  },
      new ColumnHeader { Text = "Upload (Mbps)",  Width = 120 },
      new ColumnHeader { Text = "Download (Mbps)",Width = 130 },
      new ColumnHeader { Text = "Serveur",        Width = 130 }
    ]);
  }

  // ---------------------------------------------------------------
  // Rafraîchissement de l'historique
  // ---------------------------------------------------------------

  /// <summary>
  /// Met à jour le <see cref="ListView"/> et le <see cref="Chart"/>
  /// à partir de <see cref="_history"/>.
  /// </summary>
  internal void RefreshHistory()
  {
    // ListView
    _lvHistory.Items.Clear();
    string currentServer = txtServer.Text.Trim();
    foreach (var r in _history)
    {
      var item = new ListViewItem(r.Timestamp.ToString("HH:mm:ss"));
      item.SubItems.Add(r.Upload.ToString("F2"));
      item.SubItems.Add(r.Download.ToString("F2"));
      item.SubItems.Add(currentServer);
      _lvHistory.Items.Add(item);
    }
    if (_lvHistory.Items.Count > 0)
      _lvHistory.EnsureVisible(_lvHistory.Items.Count - 1);

    // Chart
    _chart.Series["Upload"].Points.Clear();
    _chart.Series["Download"].Points.Clear();

    for (int i = 0; i < _history.Count; i++)
    {
      _chart.Series["Upload"].Points.AddXY(i + 1, _history[i].Upload);
      _chart.Series["Download"].Points.AddXY(i + 1, _history[i].Download);
    }
  }
}
