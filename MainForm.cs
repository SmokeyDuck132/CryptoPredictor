using Skender.Stock.Indicators;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace CryptoPricePredictor
{
    public partial class MainForm : Form
    {
        private TextBox txtApiKey;
        private TextBox txtApiSecret;
        private Button btnSetApiKey;
        private TextBox txtGeminiApiKey;
        private Button btnSetGeminiApiKey;
        private TextBox txtTradingPair;
        private ComboBox cmbTimeFrame;
        private Button btnFetchData;
        private Button btnPredict;
        private TextBox txtOutput;
        private TextBox txtPredictionOutput;
        private TabControl tabControl;
        private TabPage tabPrediction;
        private BybitApiService apiService;
        private GeminiApiService geminiApiService;
        private List<Quote> bybitQuotes;
        private DataGridView dgvOutput;
        private Button btnThemeToggle;
        private bool isDarkMode = false;
        private TextBox txtLineCount;
        private Label lblLineCount;
        private Button btnLiveChart;
        private TabPage tabLiveChart;
        private Chart liveChart;
        private System.Windows.Forms.Timer chartUpdateTimer;
        private bool isChartLive = false;
        private bool isDragging = false;
        private Point lastMousePosition;
        private double lastViewMinimum;
        private double lastViewMaximum;

        public MainForm()
        {
            InitializeComponent();
            InitializeLiveChartTab();
        }

        private void InitializeComponent()
        {
            this.Text = "Crypto Price Predictor";
            this.Size = new Size(1200, 1000);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Bybit API Key Label and TextBox
            var lblApiKey = new Label
            {
                Text = "Bybit API Key:",
                Location = new Point(20, 20),
                AutoSize = true
            };
            txtApiKey = new TextBox
            {
                Location = new Point(150, 20),
                Width = 300
            };

            // Bybit API Secret Label and TextBox
            var lblApiSecret = new Label
            {
                Text = "Bybit API Secret:",
                Location = new Point(20, 60),
                AutoSize = true
            };
            txtApiSecret = new TextBox
            {
                Location = new Point(150, 60),
                Width = 300,
                UseSystemPasswordChar = true
            };

            // Set Bybit API Key Button
            btnSetApiKey = new Button
            {
                Text = "Set Bybit API Key",
                Location = new Point(470, 20),
                Size = new Size(150, 30)
            };
            btnSetApiKey.Click += BtnSetApiKey_Click;

            // Gemini API Key Label and TextBox
            var lblGeminiApiKey = new Label
            {
                Text = "Gemini API Key:",
                Location = new Point(20, 100),
                AutoSize = true
            };
            txtGeminiApiKey = new TextBox
            {
                Location = new Point(150, 100),
                Width = 300
            };

            // Set Gemini API Key Button
            btnSetGeminiApiKey = new Button
            {
                Text = "Set Gemini API Key",
                Location = new Point(470, 100),
                Size = new Size(150, 30)
            };
            btnSetGeminiApiKey.Click += BtnSetGeminiApiKey_Click;

            // Trading Pair Label and TextBox
            var lblTradingPair = new Label
            {
                Text = "Trading Pair:",
                Location = new Point(20, 140),
                AutoSize = true
            };
            txtTradingPair = new TextBox
            {
                Location = new Point(150, 140),
                Width = 150
            };
            txtTradingPair.TextChanged += TxtTradingPair_TextChanged;

            // Time Frame Label and ComboBox
            var lblTimeFrame = new Label
            {
                Text = "Time Frame:",
                Location = new Point(20, 180),
                AutoSize = true
            };
            cmbTimeFrame = new ComboBox
            {
                Location = new Point(150, 180),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbTimeFrame.Items.AddRange(new string[] { "1", "5", "15", "60", "240", "D" });
            cmbTimeFrame.SelectedIndex = 3; // Default to "60"

            // Fetch Data Button
            btnFetchData = new Button
            {
                Text = "Fetch Data",
                Location = new Point(470, 140),
                Size = new Size(150, 30)
            };
            btnFetchData.Click += BtnFetchData_Click;

            // Predict Prices Button
            btnPredict = new Button
            {
                Text = "Predict Prices",
                Location = new Point(640, 140),
                Size = new Size(150, 30)
            };
            btnPredict.Click += BtnPredict_Click;

            // Number of Lines Label and TextBox
            lblLineCount = new Label
            {
                Text = "Number of lines:",
                Location = new Point(btnPredict.Right + 10, btnPredict.Top),
                AutoSize = true
            };
            txtLineCount = new TextBox
            {
                Location = new Point(lblLineCount.Right + 5, btnPredict.Top),
                Width = 50
            };

            // Theme Toggle Button
            btnThemeToggle = new Button
            {
                Text = "🌙",
                Font = new Font("Segoe UI", 12f),
                Location = new Point(1100, 20),
                Size = new Size(40, 40),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnThemeToggle.FlatAppearance.BorderSize = 0;
            btnThemeToggle.Click += BtnThemeToggle_Click;

            // ToolTip for Theme Toggle
            var toolTip = new ToolTip();
            toolTip.SetToolTip(btnThemeToggle, "Toggle Dark/Light Mode");

            // DataGridView for Output
            dgvOutput = new DataGridView
            {
                Location = new Point(20, 220),
                Size = new Size(1100, 300),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToOrderColumns = true,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells,
                ScrollBars = ScrollBars.Both,
            };

            // TabControl for Prediction
            tabControl = new TabControl
            {
                Location = new Point(20, 540),
                Size = new Size(1150, 400)
            };
            tabPrediction = new TabPage("Prediction");
            txtPredictionOutput = new TextBox
            {
                Location = new Point(20, 20),
                Width = 1050,
                Height = 350,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true
            };
            tabPrediction.Controls.Add(txtPredictionOutput);
            tabControl.TabPages.Add(tabPrediction);

            // Initialize Live Chart tab
            InitializeLiveChartTab();

            // Export Button
            var btnExport = new Button
            {
                Text = "Export",
                Location = new Point(btnPredict.Left, btnPredict.Top - 70),
                Size = new Size(150, 30)
            };
            btnExport.Click += BtnExport_Click;
            this.Controls.Add(btnExport);

            // Support Dev Button
            var btnSupportDev = new Button
            {
                Text = "Support Dev",
                Location = new Point(btnPredict.Left, btnPredict.Top - 35),
                Size = new Size(150, 30)
            };
            btnSupportDev.Click += BtnSupportDev_Click;
            this.Controls.Add(btnSupportDev);

            // Add Controls to Form
            this.Controls.Add(lblApiKey);
            this.Controls.Add(txtApiKey);
            this.Controls.Add(lblApiSecret);
            this.Controls.Add(txtApiSecret);
            this.Controls.Add(btnSetApiKey);
            this.Controls.Add(lblGeminiApiKey);
            this.Controls.Add(txtGeminiApiKey);
            this.Controls.Add(btnSetGeminiApiKey);
            this.Controls.Add(lblTradingPair);
            this.Controls.Add(txtTradingPair);
            this.Controls.Add(lblTimeFrame);
            this.Controls.Add(cmbTimeFrame);
            this.Controls.Add(btnFetchData);
            this.Controls.Add(btnPredict);
            this.Controls.Add(lblLineCount);
            this.Controls.Add(txtLineCount);
            this.Controls.Add(dgvOutput);
            this.Controls.Add(tabControl);
            this.Controls.Add(btnThemeToggle);

            // Apply Initial Theme
            ApplyTheme("Bright");
        }

        private void InitializeLiveChartTab()
        {
            Console.WriteLine("Initializing live chart tab...");

            // Create the "Live Chart" tab page
            tabLiveChart = new TabPage("Live Chart ( DEMO )");
            tabLiveChart.BackColor = Color.FromArgb(19, 19, 19);

            // Create a button to start live chart updates
            btnLiveChart = new Button
            {
                Text = "Start Live Chart",
                Location = new Point(20, 20),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLiveChart.Click += BtnLiveChart_Click;
            tabLiveChart.Controls.Add(btnLiveChart);

            // Create a Chart control with larger size
            liveChart = new Chart();
            liveChart.Location = new Point(20, 60);
            liveChart.Size = new Size(1100, 600); // Adjusted height
            liveChart.BackColor = Color.FromArgb(19, 19, 19);
            liveChart.ForeColor = Color.White;
            liveChart.AntiAliasing = AntiAliasingStyles.All;
            liveChart.TextAntiAliasingQuality = TextAntiAliasingQuality.High;

            // Enable scrolling and zooming
            liveChart.MouseWheel += LiveChart_MouseWheel;
            liveChart.MouseDown += LiveChart_MouseDown;
            liveChart.MouseMove += LiveChart_MouseMove;
            liveChart.MouseUp += LiveChart_MouseUp;

            // Configure chart areas for scrolling
            foreach (var area in liveChart.ChartAreas)
            {
                area.CursorX.IsUserEnabled = true;
                area.CursorX.IsUserSelectionEnabled = true;
                area.AxisX.ScaleView.Zoomable = true;
                area.AxisX.ScrollBar.IsPositionedInside = true;
                area.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
                area.AxisX.ScrollBar.BackColor = Color.FromArgb(30, 30, 30);
                area.AxisX.ScrollBar.ButtonColor = Color.FromArgb(60, 60, 60);
                area.AxisX.ScrollBar.LineColor = Color.FromArgb(45, 45, 45);

                area.CursorY.IsUserEnabled = true;
                area.CursorY.IsUserSelectionEnabled = true;
                area.AxisY.ScaleView.Zoomable = true;
                area.AxisY.ScrollBar.IsPositionedInside = true;
                area.AxisY.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
                area.AxisY.ScrollBar.BackColor = Color.FromArgb(30, 30, 30);
                area.AxisY.ScrollBar.ButtonColor = Color.FromArgb(60, 60, 60);
                area.AxisY.ScrollBar.LineColor = Color.FromArgb(45, 45, 45);
            }

            // Create chart areas with adjusted positions
            var priceArea = new ChartArea("PriceArea");
            ConfigurePriceChartArea(priceArea);
            liveChart.ChartAreas.Add(priceArea);

            var volumeArea = new ChartArea("VolumeArea");
            ConfigureVolumeChartArea(volumeArea, "PriceArea");
            liveChart.ChartAreas.Add(volumeArea);

            var indicatorArea = new ChartArea("IndicatorArea");
            ConfigureIndicatorChartArea(indicatorArea, "PriceArea");
            liveChart.ChartAreas.Add(indicatorArea);

            // Create series
            CreateChartSeries();

            // Add the chart to the tab
            tabLiveChart.Controls.Add(liveChart);

            // Initialize the timer
            chartUpdateTimer = new System.Windows.Forms.Timer();
            chartUpdateTimer.Interval = 1000;
            chartUpdateTimer.Tick += ChartUpdateTimer_Tick;

            // Add the tab to the TabControl
            tabControl.TabPages.Add(tabLiveChart);

            Console.WriteLine("Live chart tab initialized");
        }

        private void ConfigurePriceChartArea(ChartArea area)
        {
            // Dark theme background
            area.BackColor = Color.FromArgb(19, 19, 19);
            area.BorderColor = Color.FromArgb(26, 26, 26);
            area.BorderWidth = 1;
            area.BorderDashStyle = ChartDashStyle.Solid;

            // Grid lines
            area.AxisX.MajorGrid.LineColor = Color.FromArgb(45, 45, 45);
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Solid;
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(45, 45, 45);
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Solid;

            // Axis labels
            area.AxisX.LabelStyle.ForeColor = Color.FromArgb(130, 130, 130);
            area.AxisY.LabelStyle.ForeColor = Color.FromArgb(130, 130, 130);
            area.AxisX.LineColor = Color.FromArgb(45, 45, 45);
            area.AxisY.LineColor = Color.FromArgb(45, 45, 45);

            // Time format
            area.AxisX.LabelStyle.Format = "HH:mm";
            area.AxisX.IntervalType = DateTimeIntervalType.Minutes;
            area.AxisX.Interval = 30;

            // Layout
            area.Position.Height = 60;
            area.Position.Width = 98;
            area.Position.X = 1;
            area.Position.Y = 1;

            // Price precision
            area.AxisY.LabelStyle.Format = "0.00000";

            // Remove axis arrows
            area.AxisX.ArrowStyle = AxisArrowStyle.None;
            area.AxisY.ArrowStyle = AxisArrowStyle.None;

            // Enable scrolling and zooming
            area.CursorX.IsUserEnabled = true;
            area.CursorX.IsUserSelectionEnabled = true;
            area.AxisX.ScaleView.Zoomable = true;
            area.AxisX.ScrollBar.IsPositionedInside = true;
            area.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
            area.AxisX.ScrollBar.BackColor = Color.FromArgb(30, 30, 30);
            area.AxisX.ScrollBar.ButtonColor = Color.FromArgb(60, 60, 60);
            area.AxisX.ScrollBar.LineColor = Color.FromArgb(45, 45, 45);
        }

        private void ConfigureVolumeChartArea(ChartArea area, string alignWithArea)
        {
            area.AlignWithChartArea = alignWithArea;
            area.AxisX.LabelStyle.Enabled = false;
            area.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64);
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64);
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            area.AxisY2.Enabled = AxisEnabled.False;

            area.BackColor = Color.FromArgb(19, 23, 34);
            area.BorderColor = Color.FromArgb(26, 30, 45);
            area.BorderWidth = 1;
            area.BorderDashStyle = ChartDashStyle.Solid;

            area.Position.Height = 15;
            area.Position.Width = 98;
            area.Position.X = 1;
            area.Position.Y = 65;

            area.AxisX.LabelStyle.ForeColor = Color.LightGray;
            area.AxisY.LabelStyle.ForeColor = Color.LightGray;
        }

        private void ConfigureIndicatorChartArea(ChartArea area, string alignWithArea)
        {
            area.AlignWithChartArea = alignWithArea;
            area.AxisX.LabelStyle.Enabled = false;
            area.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64);
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64);
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            area.AxisY2.Enabled = AxisEnabled.False;

            area.BackColor = Color.FromArgb(19, 23, 34);
            area.BorderColor = Color.FromArgb(26, 30, 45);
            area.BorderWidth = 1;
            area.BorderDashStyle = ChartDashStyle.Solid;

            area.Position.Height = 15;
            area.Position.Width = 98;
            area.Position.X = 1;
            area.Position.Y = 82;

            area.AxisX.LabelStyle.ForeColor = Color.LightGray;
            area.AxisY.LabelStyle.ForeColor = Color.LightGray;
        }

        private void CreateChartSeries()
        {
            // Candlestick series
            var candleSeries = new Series("Candles");
            candleSeries.ChartType = SeriesChartType.Candlestick;
            candleSeries.ChartArea = "PriceArea";
            candleSeries.XValueType = ChartValueType.DateTime;
            candleSeries["PriceUpColor"] = "LimeGreen";           // Green candles
            candleSeries["PriceDownColor"] = "Red";               // Red candles
            candleSeries["ShowOpenClose"] = "Both";               // Show both open and close
            candleSeries.BorderWidth = 1;                         // Thin borders
            candleSeries.CustomProperties = "PriceUpColor=LimeGreen,PriceDownColor=Red";
            liveChart.Series.Add(candleSeries);

            // Moving Averages
            CreateMovingAverageSeries("MA7", "PriceArea", Color.FromArgb(70, 120, 255), 1);  // Light blue
            CreateMovingAverageSeries("MA14", "PriceArea", Color.FromArgb(255, 140, 0), 1);  // Orange
            CreateMovingAverageSeries("MA28", "PriceArea", Color.FromArgb(255, 0, 255), 1);  // Magenta

            // Volume series
            var volumeSeries = new Series("Volume");
            volumeSeries.ChartType = SeriesChartType.Column;
            volumeSeries.ChartArea = "VolumeArea";
            volumeSeries.XValueType = ChartValueType.DateTime;
            liveChart.Series.Add(volumeSeries);

            // RSI series
            CreateIndicatorSeries("RSI", "IndicatorArea", Color.LightBlue, 2);

            // MACD series
            CreateIndicatorSeries("MACD", "IndicatorArea", Color.White, 2);
            CreateIndicatorSeries("Signal", "IndicatorArea", Color.Red, 2);
            CreateIndicatorSeries("Histogram", "IndicatorArea", Color.Green, 1);
        }

        private void CreateMovingAverageSeries(string name, string chartArea, Color color, int width)
        {
            var series = new Series(name);
            series.ChartType = SeriesChartType.Line;
            series.ChartArea = chartArea;
            series.Color = color;
            series.BorderWidth = width;
            series.XValueType = ChartValueType.DateTime;
            liveChart.Series.Add(series);
        }

        private void CreateIndicatorSeries(string name, string chartArea, Color color, int width)
        {
            var series = new Series(name);
            series.ChartType = SeriesChartType.Line;
            series.ChartArea = chartArea;
            series.Color = color;
            series.BorderWidth = width;
            series.XValueType = ChartValueType.DateTime;
            liveChart.Series.Add(series);
        }

        private async void ChartUpdateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("Updating chart...");
                if (apiService == null || string.IsNullOrEmpty(txtTradingPair.Text.Trim()))
                {
                    StopLiveChart("Please set API key and trading pair first.");
                    return;
                }

                string timeframe = cmbTimeFrame.SelectedItem?.ToString() ?? "1";
                Console.WriteLine($"Fetching data for timeframe: {timeframe}");

                var klineData = await apiService.FetchKlineDataAsync(
                    "spot",
                    txtTradingPair.Text.Trim(),
                    timeframe,
                    200
                );

                if (klineData == null || !klineData.Any())
                {
                    Console.WriteLine("No data received from API");
                    return;
                }

                Console.WriteLine($"Received {klineData.Count()} candles");

                var quotes = klineData.Select(k => new Quote
                {
                    Date = DateTimeOffset.FromUnixTimeMilliseconds(k.OpenTime).UtcDateTime,
                    Open = (decimal)k.Open,
                    High = (decimal)k.High,
                    Low = (decimal)k.Low,
                    Close = (decimal)k.Close,
                    Volume = (decimal)k.Volume
                }).ToList();

                UpdateChartData(quotes);
                Console.WriteLine("Chart updated successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in timer tick: {ex}");
                StopLiveChart($"Error updating chart: {ex.Message}");
            }
        }

        private void ConfigureTimeAxis(Axis axis, string timeframe)
        {
            switch (timeframe)
            {
                case "1": // 1 minute
                    axis.LabelStyle.Format = "HH:mm";
                    axis.IntervalType = DateTimeIntervalType.Minutes;
                    axis.Interval = 5;
                    break;
                case "5": // 5 minutes
                    axis.LabelStyle.Format = "HH:mm";
                    axis.IntervalType = DateTimeIntervalType.Minutes;
                    axis.Interval = 15;
                    break;
                case "15": // 15 minutes
                    axis.LabelStyle.Format = "HH:mm";
                    axis.IntervalType = DateTimeIntervalType.Minutes;
                    axis.Interval = 30;
                    break;
                case "60": // 1 hour
                    axis.LabelStyle.Format = "HH:mm";
                    axis.IntervalType = DateTimeIntervalType.Hours;
                    axis.Interval = 1;
                    break;
                case "240": // 4 hours
                    axis.LabelStyle.Format = "MM/dd HH:mm";
                    axis.IntervalType = DateTimeIntervalType.Hours;
                    axis.Interval = 4;
                    break;
                case "D": // 1 day
                    axis.LabelStyle.Format = "MM/dd";
                    axis.IntervalType = DateTimeIntervalType.Days;
                    axis.Interval = 1;
                    break;
            }
        }

        private void UpdateChartData(List<Quote> quotes)
        {
            if (liveChart.InvokeRequired)
            {
                liveChart.Invoke(new Action(() => UpdateChartData(quotes)));
                return;
            }

            try
            {
                Console.WriteLine("Updating chart data...");

                // Clear existing points
                foreach (var series in liveChart.Series)
                {
                    series.Points.Clear();
                }

                if (!quotes.Any())
                {
                    Console.WriteLine("No quotes to display");
                    return;
                }

                // Calculate all indicators
                var ma7 = quotes.GetSma(7).ToList();
                var ma14 = quotes.GetSma(14).ToList();
                var ma28 = quotes.GetSma(28).ToList();
                var rsi = quotes.GetRsi(14).ToList();
                var macd = quotes.GetMacd(12, 26, 9).ToList();

                // Update candlesticks and indicators
                foreach (var quote in quotes)
                {
                    // Add candlestick
                    var point = new DataPoint();
                    point.XValue = quote.Date.ToOADate();
                    point.YValues = new double[] {
                        (double)quote.High,
                        (double)quote.Low,
                        (double)quote.Open,
                        (double)quote.Close
                    };
                    liveChart.Series["Candles"].Points.Add(point);

                    // Add volume
                    var volumePoint = new DataPoint();
                    volumePoint.XValue = quote.Date.ToOADate();
                    volumePoint.YValues = new double[] { (double)quote.Volume };
                    volumePoint.Color = quote.Close >= quote.Open ? Color.LimeGreen : Color.Red;
                    liveChart.Series["Volume"].Points.Add(volumePoint);

                    // Update all indicators
                    UpdateIndicators(quote, ma7, ma14, ma28, rsi, macd);
                }

                // Auto-scale the chart
                foreach (var area in liveChart.ChartAreas)
                {
                    area.RecalculateAxesScale();
                }

                Console.WriteLine($"Updated chart with {quotes.Count} quotes");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating chart data: {ex}");
                MessageBox.Show($"Error updating chart: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateIndicators(Quote quote, List<SmaResult> ma7, List<SmaResult> ma14,
            List<SmaResult> ma28, List<RsiResult> rsi, List<MacdResult> macd)
        {
            // Update Moving Averages
            if (ma7.Any())
            {
                var ma7Point = ma7.FirstOrDefault(x => x.Date == quote.Date);
                if (ma7Point != null)
                    liveChart.Series["MA7"].Points.AddXY(quote.Date, ma7Point.Sma);
            }

            if (ma14.Any())
            {
                var ma14Point = ma14.FirstOrDefault(x => x.Date == quote.Date);
                if (ma14Point != null)
                    liveChart.Series["MA14"].Points.AddXY(quote.Date, ma14Point.Sma);
            }

            if (ma28.Any())
            {
                var ma28Point = ma28.FirstOrDefault(x => x.Date == quote.Date);
                if (ma28Point != null)
                    liveChart.Series["MA28"].Points.AddXY(quote.Date, ma28Point.Sma);
            }

            // Update RSI
            if (rsi.Any())
            {
                var rsiPoint = rsi.FirstOrDefault(x => x.Date == quote.Date);
                if (rsiPoint != null)
                    liveChart.Series["RSI"].Points.AddXY(quote.Date, rsiPoint.Rsi);
            }

            // Update MACD
            var macdPoint = macd.FirstOrDefault(x => x.Date == quote.Date);
            if (macdPoint != null)
            {
                liveChart.Series["MACD"].Points.AddXY(quote.Date, macdPoint.Macd);
                liveChart.Series["Signal"].Points.AddXY(quote.Date, macdPoint.Signal);
                liveChart.Series["Histogram"].Points.AddXY(quote.Date, macdPoint.Histogram);
            }
        }

        private void StopLiveChart(string errorMessage)
        {
            chartUpdateTimer.Stop();
            btnLiveChart.Text = "Start Live Chart";
            isChartLive = false;
            MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void BtnThemeToggle_Click(object sender, EventArgs e)
        {
            isDarkMode = !isDarkMode;
            btnThemeToggle.Text = isDarkMode ? "☀️" : "🌙";
            ApplyTheme(isDarkMode ? "Dark" : "Bright");
        }

        private void ApplyTheme(string theme)
        {
            if (theme == "Dark")
            {
                this.BackColor = Color.FromArgb(32, 32, 32);
                Color darkBackground = Color.FromArgb(45, 45, 48);
                Color darkControl = Color.FromArgb(28, 28, 28);
                Color darkText = Color.FromArgb(240, 240, 240);
                Color borderColor = Color.FromArgb(61, 61, 61);

                foreach (Control control in this.Controls)
                {
                    if (control is Label)
                    {
                        control.ForeColor = darkText;
                        control.BackColor = Color.Transparent;
                    }
                    else if (control is Button btn)
                    {
                        btn.FlatStyle = FlatStyle.Flat;
                        btn.FlatAppearance.BorderColor = borderColor;
                        btn.BackColor = darkControl;
                        btn.ForeColor = darkText;
                        btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60);
                        btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(80, 80, 80);
                    }
                    else if (control is TextBox txt)
                    {
                        txt.BackColor = darkControl;
                        txt.ForeColor = darkText;
                        txt.BorderStyle = BorderStyle.FixedSingle;
                    }
                    else if (control is ComboBox cmb)
                    {
                        cmb.BackColor = darkControl;
                        cmb.ForeColor = darkText;
                        cmb.FlatStyle = FlatStyle.Flat;
                    }
                }

                dgvOutput.BackgroundColor = darkBackground;
                dgvOutput.ForeColor = darkText;
                dgvOutput.GridColor = borderColor;
                dgvOutput.DefaultCellStyle.BackColor = darkControl;
                dgvOutput.DefaultCellStyle.ForeColor = darkText;
                dgvOutput.ColumnHeadersDefaultCellStyle.BackColor = darkBackground;
                dgvOutput.ColumnHeadersDefaultCellStyle.ForeColor = darkText;
                dgvOutput.EnableHeadersVisualStyles = false;

                tabControl.BackColor = darkBackground;

                foreach (TabPage page in tabControl.TabPages)
                {
                    page.BackColor = darkBackground;
                    page.ForeColor = darkText;
                }

                txtPredictionOutput.BackColor = darkControl;
                txtPredictionOutput.ForeColor = darkText;

                btnThemeToggle.BackColor = Color.FromArgb(45, 45, 48);
                btnThemeToggle.ForeColor = Color.FromArgb(240, 240, 240);
                btnThemeToggle.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 60, 60);
                btnThemeToggle.FlatAppearance.MouseDownBackColor = Color.FromArgb(80, 80, 80);

                // Live Chart styling
                if (tabLiveChart != null)
                {
                    tabLiveChart.BackColor = darkBackground;
                    btnLiveChart.BackColor = darkControl;
                    btnLiveChart.ForeColor = darkText;
                    liveChart.BackColor = darkBackground;
                    foreach (var area in liveChart.ChartAreas)
                    {
                        area.BackColor = darkBackground;
                        area.AxisX.LabelStyle.ForeColor = darkText;
                        area.AxisY.LabelStyle.ForeColor = darkText;
                        area.AxisX.LineColor = darkText;
                        area.AxisY.LineColor = darkText;
                        area.AxisX.MajorGrid.LineColor = Color.FromArgb(70, 70, 70);
                        area.AxisY.MajorGrid.LineColor = Color.FromArgb(70, 70, 70);
                    }
                }
            }
            else
            {
                this.BackColor = Color.White;
                Color lightBackground = Color.FromArgb(250, 250, 250);
                Color lightControl = Color.White;
                Color lightText = Color.FromArgb(30, 30, 30);
                Color borderColor = Color.FromArgb(217, 217, 217);

                foreach (Control control in this.Controls)
                {
                    if (control is Label)
                    {
                        control.ForeColor = lightText;
                        control.BackColor = Color.Transparent;
                    }
                    else if (control is Button btn)
                    {
                        btn.FlatStyle = FlatStyle.Standard;
                        btn.BackColor = lightControl;
                        btn.ForeColor = lightText;
                    }
                    else if (control is TextBox txt)
                    {
                        txt.BackColor = lightControl;
                        txt.ForeColor = lightText;
                        txt.BorderStyle = BorderStyle.Fixed3D;
                    }
                    else if (control is ComboBox cmb)
                    {
                        cmb.BackColor = lightControl;
                        cmb.ForeColor = lightText;
                        cmb.FlatStyle = FlatStyle.Standard;
                    }
                }

                dgvOutput.BackgroundColor = lightBackground;
                dgvOutput.ForeColor = lightText;
                dgvOutput.GridColor = borderColor;
                dgvOutput.DefaultCellStyle.BackColor = lightControl;
                dgvOutput.DefaultCellStyle.ForeColor = lightText;
                dgvOutput.ColumnHeadersDefaultCellStyle.BackColor = lightBackground;
                dgvOutput.ColumnHeadersDefaultCellStyle.ForeColor = lightText;
                dgvOutput.EnableHeadersVisualStyles = true;

                tabControl.BackColor = lightBackground;

                foreach (TabPage page in tabControl.TabPages)
                {
                    page.BackColor = lightBackground;
                    page.ForeColor = lightText;
                }

                txtPredictionOutput.BackColor = lightControl;
                txtPredictionOutput.ForeColor = lightText;

                btnThemeToggle.BackColor = Color.White;
                btnThemeToggle.ForeColor = Color.FromArgb(30, 30, 30);
                btnThemeToggle.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
                btnThemeToggle.FlatAppearance.MouseDownBackColor = Color.FromArgb(230, 230, 230);

                // Live Chart styling
                if (tabLiveChart != null)
                {
                    tabLiveChart.BackColor = lightBackground;
                    btnLiveChart.BackColor = lightControl;
                    btnLiveChart.ForeColor = lightText;
                    liveChart.BackColor = lightBackground;
                    foreach (var area in liveChart.ChartAreas)
                    {
                        area.BackColor = lightBackground;
                        area.AxisX.LabelStyle.ForeColor = lightText;
                        area.AxisY.LabelStyle.ForeColor = lightText;
                        area.AxisX.LineColor = lightText;
                        area.AxisY.LineColor = lightText;
                        area.AxisX.MajorGrid.LineColor = Color.LightGray;
                        area.AxisY.MajorGrid.LineColor = Color.LightGray;
                    }
                }
            }
        }

        private async void BtnFetchData_Click(object sender, EventArgs e)
        {
            if (apiService == null)
            {
                MessageBox.Show("Please set Bybit API key and secret first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var symbol = txtTradingPair.Text.Trim();
                var interval = cmbTimeFrame.SelectedItem?.ToString() ?? "60";
                var klineData = await apiService.FetchKlineDataAsync("spot", symbol, interval, 200);

                bybitQuotes = klineData.Select(k => new Quote
                {
                    Date = DateTimeOffset.FromUnixTimeMilliseconds(k.OpenTime).UtcDateTime,
                    Open = (decimal)k.Open,
                    High = (decimal)k.High,
                    Low = (decimal)k.Low,
                    Close = (decimal)k.Close,
                    Volume = (decimal)k.Volume
                }).ToList();

                PopulateDataGridView(bybitQuotes);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateDataGridView(List<Quote> quotes)
        {
            dgvOutput.Columns.Clear();
            dgvOutput.Rows.Clear();
            var columns = new[] { "Time", "Open", "High", "Low", "Close", "Volume", "MA7", "MA14", "MA28", "EMA7", "EMA14", "EMA28", "BOLL_LOW", "BOLL_HIGH", "SAR", "RSI6", "RSI12", "RSI24", "StochRSI", "MACD", "KDJ" };
            foreach (var col in columns)
                dgvOutput.Columns.Add(col, col);

            var vietnamTimeOffset = TimeSpan.FromHours(7);

            var ma7 = quotes.GetSma(7).ToList();
            var ma14 = quotes.GetSma(14).ToList();
            var ma28 = quotes.GetSma(28).ToList();
            var ema7 = quotes.GetEma(7).ToList();
            var ema14 = quotes.GetEma(14).ToList();
            var ema28 = quotes.GetEma(28).ToList();
            var boll = quotes.GetBollingerBands(20, 2).ToList();
            var sar = quotes.GetParabolicSar(0.02, 0.2).ToList();
            var rsi6 = quotes.GetRsi(6).ToList();
            var rsi12 = quotes.GetRsi(12).ToList();
            var rsi24 = quotes.GetRsi(24).ToList();
            var stochRsi = quotes.GetStochRsi(14, 14, 3, 3).ToList();
            var macd = quotes.GetMacd(12, 26, 9).ToList();
            var kdj = quotes.GetStoch(9, 3, 3).ToList();

            foreach (var q in quotes)
            {
                var adjustedTime = q.Date.Add(vietnamTimeOffset);
                dgvOutput.Rows.Add(
                    adjustedTime.ToString("yyyy-MM-dd HH:mm"),
                    Math.Round(q.Open, 2),
                    Math.Round(q.High, 2),
                    Math.Round(q.Low, 2),
                    Math.Round(q.Close, 2),
                    Math.Round(q.Volume, 2),
                    Math.Round(ma7.FirstOrDefault(x => x.Date == q.Date)?.Sma ?? 0, 2),
                    Math.Round(ma14.FirstOrDefault(x => x.Date == q.Date)?.Sma ?? 0, 2),
                    Math.Round(ma28.FirstOrDefault(x => x.Date == q.Date)?.Sma ?? 0, 2),
                    Math.Round(ema7.FirstOrDefault(x => x.Date == q.Date)?.Ema ?? 0, 2),
                    Math.Round(ema14.FirstOrDefault(x => x.Date == q.Date)?.Ema ?? 0, 2),
                    Math.Round(ema28.FirstOrDefault(x => x.Date == q.Date)?.Ema ?? 0, 2),
                    Math.Round(boll.FirstOrDefault(x => x.Date == q.Date)?.LowerBand ?? 0, 2),
                    Math.Round(boll.FirstOrDefault(x => x.Date == q.Date)?.UpperBand ?? 0, 2),
                    Math.Round(sar.FirstOrDefault(x => x.Date == q.Date)?.Sar ?? 0, 2),
                    Math.Round(rsi6.FirstOrDefault(x => x.Date == q.Date)?.Rsi ?? 0, 2),
                    Math.Round(rsi12.FirstOrDefault(x => x.Date == q.Date)?.Rsi ?? 0, 2),
                    Math.Round(rsi24.FirstOrDefault(x => x.Date == q.Date)?.Rsi ?? 0, 2),
                    Math.Round(stochRsi.FirstOrDefault(x => x.Date == q.Date)?.StochRsi ?? 0, 2),
                    Math.Round(macd.FirstOrDefault(x => x.Date == q.Date)?.Macd ?? 0, 2),
                    Math.Round(kdj.FirstOrDefault(x => x.Date == q.Date)?.D ?? 0, 2)
                );
            }
        }

        private async void BtnPredict_Click(object sender, EventArgs e)
        {
            if (geminiApiService == null || bybitQuotes == null || !bybitQuotes.Any())
            {
                MessageBox.Show("Please ensure Gemini API is set and data is fetched.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtLineCount.Text))
            {
                MessageBox.Show("Please enter the number of lines to send.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtLineCount.Text.Trim(), out int lineCount) || lineCount <= 0)
            {
                MessageBox.Show("Please enter a valid number of lines to send.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (lineCount > bybitQuotes.Count)
            {
                MessageBox.Show($"You have only {bybitQuotes.Count} data points available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            txtPredictionOutput.Text = "Generating prediction... Please wait.";

            try
            {
                var inputData = new StringBuilder();
                inputData.AppendLine("Time, Open, High, Low, Close, Volume, MA7, MA14, MA28, EMA7, EMA14, EMA28, BOLL_LOW, BOLL_HIGH, SAR, RSI6, RSI12, RSI24, StochRSI, MACD, KDJ");

                var vietnamTimeOffset = TimeSpan.FromHours(7);

                var limitedQuotes = bybitQuotes.Take(lineCount).ToList();

                var ma7 = limitedQuotes.GetSma(7).ToList();
                var ma14 = limitedQuotes.GetSma(14).ToList();
                var ma28 = limitedQuotes.GetSma(28).ToList();
                var ema7 = limitedQuotes.GetEma(7).ToList();
                var ema14 = limitedQuotes.GetEma(14).ToList();
                var ema28 = limitedQuotes.GetEma(28).ToList();
                var boll = limitedQuotes.GetBollingerBands(20, 2).ToList();
                var sar = limitedQuotes.GetParabolicSar(0.02, 0.2).ToList();
                var rsi6 = limitedQuotes.GetRsi(6).ToList();
                var rsi12 = limitedQuotes.GetRsi(12).ToList();
                var rsi24 = limitedQuotes.GetRsi(24).ToList();
                var stochRsi = limitedQuotes.GetStochRsi(14, 14, 3, 3).ToList();
                var macd = limitedQuotes.GetMacd(12, 26, 9).ToList();
                var kdj = limitedQuotes.GetStoch(9, 3, 3).ToList();

                foreach (var q in limitedQuotes)
                {
                    var adjustedTime = q.Date.Add(vietnamTimeOffset);
                    inputData.AppendLine($"{adjustedTime:yyyy-MM-dd HH:mm}, {q.Open:F2}, {q.High:F2}, {q.Low:F2}, {q.Close:F2}, {q.Volume:F2}, " +
                                         $"{ma7.FirstOrDefault(x => x.Date == q.Date)?.Sma ?? 0:F2}, " +
                                         $"{ma14.FirstOrDefault(x => x.Date == q.Date)?.Sma ?? 0:F2}, " +
                                         $"{ma28.FirstOrDefault(x => x.Date == q.Date)?.Sma ?? 0:F2}, " +
                                         $"{ema7.FirstOrDefault(x => x.Date == q.Date)?.Ema ?? 0:F2}, " +
                                         $"{ema14.FirstOrDefault(x => x.Date == q.Date)?.Ema ?? 0:F2}, " +
                                         $"{ema28.FirstOrDefault(x => x.Date == q.Date)?.Ema ?? 0:F2}, " +
                                         $"{boll.FirstOrDefault(x => x.Date == q.Date)?.LowerBand ?? 0:F2}, " +
                                         $"{boll.FirstOrDefault(x => x.Date == q.Date)?.UpperBand ?? 0:F2}, " +
                                         $"{sar.FirstOrDefault(x => x.Date == q.Date)?.Sar ?? 0:F2}, " +
                                         $"{rsi6.FirstOrDefault(x => x.Date == q.Date)?.Rsi ?? 0:F2}, " +
                                         $"{rsi12.FirstOrDefault(x => x.Date == q.Date)?.Rsi ?? 0:F2}, " +
                                         $"{rsi24.FirstOrDefault(x => x.Date == q.Date)?.Rsi ?? 0:F2}, " +
                                         $"{stochRsi.FirstOrDefault(x => x.Date == q.Date)?.StochRsi ?? 0:F2}, " +
                                         $"{macd.FirstOrDefault(x => x.Date == q.Date)?.Macd ?? 0:F2}, " +
                                         $"{kdj.FirstOrDefault(x => x.Date == q.Date)?.D ?? 0:F2}");
                }

                var prediction = await geminiApiService.GetPredictionAsync(inputData.ToString());
                txtPredictionOutput.Text = prediction;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating prediction: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSetApiKey_Click(object sender, EventArgs e)
        {
            var apiKey = txtApiKey.Text.Trim();
            var apiSecret = txtApiSecret.Text.Trim();

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
            {
                MessageBox.Show("Please provide both API Key and Secret.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            apiService = new BybitApiService(apiKey, apiSecret);
            MessageBox.Show("Bybit API Key set successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSetGeminiApiKey_Click(object sender, EventArgs e)
        {
            var geminiApiKey = txtGeminiApiKey.Text.Trim();

            if (string.IsNullOrEmpty(geminiApiKey))
            {
                MessageBox.Show("Please provide a Gemini API Key.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            geminiApiService = new GeminiApiService(geminiApiKey);
            MessageBox.Show("Gemini API Key set successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnLiveChart_Click(object sender, EventArgs e)
        {
            try
            {
                if (isChartLive)
                {
                    Console.WriteLine("Stopping live chart...");
                    chartUpdateTimer.Stop();
                    btnLiveChart.Text = "Start Live Chart";
                    isChartLive = false;
                }
                else
                {
                    Console.WriteLine("Starting live chart...");
                    // Check requirements
                    if (apiService == null)
                    {
                        MessageBox.Show("Please set Bybit API key first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (string.IsNullOrEmpty(txtTradingPair.Text.Trim()))
                    {
                        MessageBox.Show("Please enter a trading pair.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Force initial update
                    ChartUpdateTimer_Tick(null, null);

                    // Start timer for subsequent updates
                    chartUpdateTimer.Start();
                    btnLiveChart.Text = "Stop Live Chart";
                    isChartLive = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting live chart: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"Error: {ex}");
                chartUpdateTimer.Stop();
                btnLiveChart.Text = "Start Live Chart";
                isChartLive = false;
            }
        }

        private int GetTimerInterval(string timeframe)
        {
            // Return interval in milliseconds
            switch (timeframe)
            {
                case "1": return 1000;  // 1 second for 1m
                case "5": return 5000;  // 5 seconds for 5m
                case "15": return 15000;  // 15 seconds for 15m
                case "60": return 60000;  // 1 minute for 1h
                case "240": return 240000;  // 4 minutes for 4h
                case "D": return 300000;  // 5 minutes for 1d
                default: return 1000;
            }
        }

        private void LiveChart_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                // Get the chart area
                ChartArea area = liveChart.ChartAreas["PriceArea"];

                // Calculate zoom factor
                double zoomFactor = (e.Delta < 0) ? 1.1 : 0.9;

                // Get the cursor position in axis coordinates
                double posXStart = area.AxisX.PixelPositionToValue(e.Location.X);

                // Calculate new axis limits
                double newWidth = (area.AxisX.ScaleView.ViewMaximum - area.AxisX.ScaleView.ViewMinimum) * zoomFactor;
                double newStart = posXStart - (posXStart - area.AxisX.ScaleView.ViewMinimum) * zoomFactor;
                double newEnd = newStart + newWidth;

                // Apply new values
                area.AxisX.ScaleView.Zoom(newStart, newEnd);

                // Sync other chart areas
                SyncChartAreas(area);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Zoom error: {ex.Message}");
            }
        }

        private void LiveChart_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastMousePosition = e.Location;
                lastViewMinimum = liveChart.ChartAreas["PriceArea"].AxisX.ScaleView.ViewMinimum;
                lastViewMaximum = liveChart.ChartAreas["PriceArea"].AxisX.ScaleView.ViewMaximum;
                liveChart.Cursor = Cursors.Hand;
            }
        }

        private void LiveChart_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                ChartArea area = liveChart.ChartAreas["PriceArea"];

                // Calculate the movement in axis coordinates
                double moveXPixels = e.Location.X - lastMousePosition.X;
                double moveX = area.AxisX.PixelPositionToValue(e.Location.X) -
                              area.AxisX.PixelPositionToValue(lastMousePosition.X);

                // Apply the movement
                double newViewMinimum = lastViewMinimum - moveX;
                double newViewMaximum = lastViewMaximum - moveX;

                area.AxisX.ScaleView.Zoom(newViewMinimum, newViewMaximum);

                // Sync other chart areas
                SyncChartAreas(area);
            }
        }

        private void LiveChart_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
            liveChart.Cursor = Cursors.Default;
        }

        private void SyncChartAreas(ChartArea sourceArea)
        {
            // Sync all chart areas with the source area
            foreach (ChartArea area in liveChart.ChartAreas)
            {
                if (area != sourceArea)
                {
                    area.AxisX.ScaleView.Zoom(
                        sourceArea.AxisX.ScaleView.ViewMinimum,
                        sourceArea.AxisX.ScaleView.ViewMaximum
                    );
                }
            }
        }

        private void BtnSupportDev_Click(object sender, EventArgs e)
        {
            try
            {
                var imagePath = "dev.jpg"; // Ensure this path is correct
                var image = Image.FromFile(imagePath);
                var form = new Form
                {
                    Text = "Support Dev",
                    Size = new Size(image.Width + 20, image.Height + 40)
                };
                var pictureBox = new PictureBox
                {
                    Image = image,
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.StretchImage
                };
                form.Controls.Add(pictureBox);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                if (bybitQuotes == null || !bybitQuotes.Any())
                {
                    MessageBox.Show("No data available to export.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Word Document|*.docx|Excel Workbook|*.xlsx",
                    Title = "Export Data"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var filePath = saveFileDialog.FileName;
                    if (filePath.EndsWith(".docx"))
                    {
                        ExportToWord(filePath);
                    }
                    else if (filePath.EndsWith(".xlsx"))
                    {
                        ExportToExcel(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToWord(string filePath)
        {
            // Implement Word export logic here
            // You can use libraries like DocX or OpenXML SDK
            MessageBox.Show("Export to Word is not yet implemented.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ExportToExcel(string filePath)
        {
            // Implement Excel export logic here
            // You can use libraries like EPPlus or ClosedXML
            MessageBox.Show("Export to Excel is not yet implemented.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TxtTradingPair_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                int selectionStart = textBox.SelectionStart;
                textBox.Text = textBox.Text.ToUpper();
                textBox.SelectionStart = selectionStart;
            }
        }
    }
}
