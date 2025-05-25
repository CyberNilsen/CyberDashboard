using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using CyberDashboard;

namespace CyberDashboard
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private Button currentActiveButton;
        private SystemMonitorService systemMonitor;
        private SystemStats currentStats;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            InitializeSystemMonitor();
            SetActiveButton(DashboardBtn);
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void InitializeSystemMonitor()
        {
            systemMonitor = new SystemMonitorService();
            systemMonitor.StatsUpdated += OnStatsUpdated;
        }

        private void OnStatsUpdated(object sender, SystemStats stats)
        {
            currentStats = stats;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateDashboardStats();
                UpdateStatusInfo();
            }));
        }

        private void UpdateDashboardStats()
        {
            if (currentStats == null) return;

            var dashboardContent = MainContent.Content as ScrollViewer;
            if (dashboardContent?.Content is StackPanel panel)
            {
                UpdateDashboardCards(panel);
            }
        }

        private void UpdateDashboardCards(StackPanel panel)
        {
            foreach (var child in panel.Children)
            {
                if (child is UniformGrid grid && grid.Children.Count == 4)
                {
                    if (grid.Children[0] is Border cpuCard)
                        UpdateCardValue(cpuCard, $"{currentStats.CpuUsage}%");

                    if (grid.Children[1] is Border gpuCard)
                        UpdateCardValue(gpuCard, $"{currentStats.GpuInfo.Usage}%");

                    if (grid.Children[2] is Border ramCard)
                        UpdateCardValue(ramCard, $"{currentStats.MemoryInfo.UsedGB} GB");

                    if (grid.Children[3] is Border storageCard && currentStats.StorageInfo.Length > 0)
                        UpdateCardValue(storageCard, $"{currentStats.StorageInfo[0].UsedGB} GB");

                    break;
                }
            }
        }

        private void UpdateCardValue(Border card, string newValue)
        {
            if (card.Child is StackPanel cardPanel)
            {
                foreach (var child in cardPanel.Children)
                {
                    if (child is TextBlock textBlock && textBlock.FontSize == 24)
                    {
                        textBlock.Text = newValue;
                        break;
                    }
                }
            }
        }

        private void UpdateStatusInfo()
        {
            if (currentStats?.SystemInfo != null)
            {
                StatusText.Text = $"Uptime: {currentStats.SystemInfo.Uptime} | CPU: {currentStats.CpuUsage}% | RAM: {currentStats.MemoryInfo.UsagePercentage}%";
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            CurrentTime.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            timer?.Stop();
            systemMonitor?.Dispose();
            this.Close();
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
                MaximizeIcon.Text = "\uE923";
            }
            else
            {
                this.WindowState = WindowState.Normal;
                MaximizeIcon.Text = "\uE922";
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                SetActiveButton(clickedButton);
                string page = clickedButton.Tag?.ToString();
                LoadPage(page);
            }
        }

        private void SetActiveButton(Button activeButton)
        {
            if (currentActiveButton != null)
            {
                currentActiveButton.Style = (Style)FindResource("NavButton");
            }

            currentActiveButton = activeButton;
            if (currentActiveButton != null)
            {
                currentActiveButton.Style = (Style)FindResource("ActiveNavButton");
            }
        }

        private void LoadPage(string pageName)
        {
            switch (pageName)
            {
                case "Dashboard":
                    PageTitle.Text = "Dashboard Overview";
                    PageStatus.Text = "ACTIVE";
                    LoadDashboardContent();
                    break;
                case "CPU":
                    PageTitle.Text = "CPU Monitoring";
                    PageStatus.Text = "MONITORING";
                    LoadCpuContent();
                    break;
                case "GPU":
                    PageTitle.Text = "GPU Monitoring";
                    PageStatus.Text = "MONITORING";
                    LoadGpuContent();
                    break;
                case "Memory":
                    PageTitle.Text = "Memory Usage";
                    PageStatus.Text = "MONITORING";
                    LoadMemoryContent();
                    break;
                case "Storage":
                    PageTitle.Text = "Storage Analysis";
                    PageStatus.Text = "MONITORING";
                    LoadStorageContent();
                    break;
                case "Network":
                    PageTitle.Text = "Network Monitor";
                    PageStatus.Text = "MONITORING";
                    LoadNetworkContent();
                    break;
                case "Security":
                    PageTitle.Text = "Security Center";
                    PageStatus.Text = "PROTECTED";
                    LoadSecurityContent();
                    break;
                case "Processes":
                    PageTitle.Text = "Process Manager";
                    PageStatus.Text = "MONITORING";
                    LoadProcessesContent();
                    break;
                case "Settings":
                    PageTitle.Text = "System Settings";
                    PageStatus.Text = "CONFIG";
                    LoadSettingsContent();
                    break;
                default:
                    LoadDashboardContent();
                    break;
            }
        }

        private void LoadDashboardContent()
        {
            var content = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Padding = new Thickness(20) };
            var panel = new StackPanel();

            var header = new TextBlock
            {
                Text = "System Overview",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 0, 0, 20)
            };
            panel.Children.Add(header);

            var grid = new UniformGrid { Columns = 4, Margin = new Thickness(0, 0, 0, 30) };

            var cpuUsage = currentStats?.CpuUsage.ToString("F1") + "%" ?? "Loading...";
            var gpuUsage = currentStats?.GpuInfo.Usage.ToString("F1") + "%" ?? "Loading...";
            var memoryUsed = currentStats?.MemoryInfo.UsedGB.ToString("F1") + " GB" ?? "Loading...";
            var storageUsed = currentStats?.StorageInfo?[0]?.UsedGB.ToString("F1") + " GB" ?? "Loading...";

            var cpuCard = CreateInfoCard("CPU", cpuUsage, "#00FF88", "\uE9D9");
            var gpuCard = CreateInfoCard("GPU", gpuUsage, "#FF6B35", "\uE7F4");
            var ramCard = CreateInfoCard("RAM", memoryUsed, "#A855F7", "\uE950");
            var storageCard = CreateInfoCard("Storage", storageUsed, "#F59E0B", "\uEDA2");

            grid.Children.Add(cpuCard);
            grid.Children.Add(gpuCard);
            grid.Children.Add(ramCard);
            grid.Children.Add(storageCard);
            panel.Children.Add(grid);

            var monitorHeader = new TextBlock
            {
                Text = "Real-time Monitoring",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 20, 0, 20)
            };
            panel.Children.Add(monitorHeader);

            var infoPanel = CreateSystemInfoPanel();
            panel.Children.Add(infoPanel);

            content.Content = panel;
            MainContent.Content = content;
        }

        private Grid CreateSystemInfoPanel()
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var chartCard = new Border
            {
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E1E1E")),
                CornerRadius = new CornerRadius(12),
                Margin = new Thickness(0, 0, 10, 0),
                Height = 300,
                Padding = new Thickness(20)
            };

            var chartPanel = new StackPanel();
            var chartTitle = new TextBlock
            {
                Text = "Performance Overview",
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 0, 0, 20)
            };
            chartPanel.Children.Add(chartTitle);

            var chartArea = new Border
            {
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#0F0F0F")),
                CornerRadius = new CornerRadius(8),
                Height = 240
            };

            var chartText = new TextBlock
            {
                Text = "📊 Live Performance Charts",
                FontSize = 16,
                Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#666666")),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            chartArea.Child = chartText;
            chartPanel.Children.Add(chartArea);
            chartCard.Child = chartPanel;

            Grid.SetColumn(chartCard, 0);
            grid.Children.Add(chartCard);

            var infoCard = new Border
            {
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E1E1E")),
                CornerRadius = new CornerRadius(12),
                Margin = new Thickness(10, 0, 0, 0),
                Height = 300,
                Padding = new Thickness(20)
            };

            var infoPanel = new StackPanel();
            var infoTitle = new TextBlock
            {
                Text = "System Information",
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 0, 0, 20)
            };
            infoPanel.Children.Add(infoTitle);

            if (currentStats?.SystemInfo != null)
            {
                infoPanel.Children.Add(CreateInfoItem("Uptime", currentStats.SystemInfo.Uptime));
                infoPanel.Children.Add(CreateInfoItem("Temperature", $"CPU: {currentStats.CpuTemperature}°C | GPU: {currentStats.GpuInfo.Temperature}°C"));
                infoPanel.Children.Add(CreateInfoItem("Network", $"↓ {currentStats.NetworkInfo.DownloadSpeed} Mbps | ↑ {currentStats.NetworkInfo.UploadSpeed} Mbps"));
                infoPanel.Children.Add(CreateInfoItem("Active Processes", $"{currentStats.ProcessInfo.RunningProcesses} running"));
            }
            else
            {
                infoPanel.Children.Add(CreateInfoItem("Uptime", "Loading..."));
                infoPanel.Children.Add(CreateInfoItem("Temperature", "Loading..."));
                infoPanel.Children.Add(CreateInfoItem("Network", "Loading..."));
                infoPanel.Children.Add(CreateInfoItem("Active Processes", "Loading..."));
            }

            infoCard.Child = infoPanel;
            Grid.SetColumn(infoCard, 1);
            grid.Children.Add(infoCard);

            return grid;
        }

        private StackPanel CreateInfoItem(string label, string value)
        {
            var panel = new StackPanel { Margin = new Thickness(0, 0, 0, 15) };

            var labelBlock = new TextBlock
            {
                Text = label,
                FontSize = 10,
                Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#999999"))
            };

            var valueBlock = new TextBlock
            {
                Text = value,
                FontSize = 12,
                Foreground = System.Windows.Media.Brushes.White
            };

            panel.Children.Add(labelBlock);
            panel.Children.Add(valueBlock);

            return panel;
        }

        private Border CreateInfoCard(string title, string value, string color, string icon)
        {
            var card = new Border
            {
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E1E1E")),
                CornerRadius = new CornerRadius(12),
                Margin = new Thickness(0, 0, 10, 0),
                Padding = new Thickness(20)
            };

            var panel = new StackPanel();

            var headerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var iconBlock = new TextBlock
            {
                Text = icon,
                FontFamily = new System.Windows.Media.FontFamily("Segoe MDL2 Assets"),
                FontSize = 20,
                Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color))
            };

            var titleBlock = new TextBlock
            {
                Text = title,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(10, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            headerPanel.Children.Add(iconBlock);
            headerPanel.Children.Add(titleBlock);

            var valueBlock = new TextBlock
            {
                Text = value,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color))
            };

            panel.Children.Add(headerPanel);
            panel.Children.Add(valueBlock);

            card.Child = panel;
            return card;
        }

        private void LoadCpuContent()
        {
            var content = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Padding = new Thickness(20) };
            var panel = new StackPanel();

            var header = new TextBlock
            {
                Text = "CPU Performance Monitoring",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 0, 0, 20)
            };
            panel.Children.Add(header);

            var overviewCard = new Border
            {
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E1E1E")),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20),
                Margin = new Thickness(0, 0, 0, 20)
            };

            var overviewPanel = new StackPanel();
            var cpuName = currentStats?.SystemInfo?.ProcessorName ?? "Loading...";
            var cpuUsage = currentStats?.CpuUsage.ToString("F1") + "%" ?? "0%";
            var cpuTemp = currentStats?.CpuTemperature.ToString("F1") + "°C" ?? "0°C";

            overviewPanel.Children.Add(new TextBlock { Text = "CPU Overview", FontSize = 16, FontWeight = FontWeights.SemiBold, Foreground = System.Windows.Media.Brushes.White, Margin = new Thickness(0, 0, 0, 15) });
            overviewPanel.Children.Add(CreateInfoItem("Processor", cpuName));
            overviewPanel.Children.Add(CreateInfoItem("Current Usage", cpuUsage));
            overviewPanel.Children.Add(CreateInfoItem("Temperature", cpuTemp));

            overviewCard.Child = overviewPanel;
            panel.Children.Add(overviewCard);

            if (currentStats?.CpuCoreUsages != null && currentStats.CpuCoreUsages.Length > 0)
            {
                var coresHeader = new TextBlock
                {
                    Text = "CPU Core Usage",
                    FontSize = 16,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = System.Windows.Media.Brushes.White,
                    Margin = new Thickness(0, 20, 0, 15)
                };
                panel.Children.Add(coresHeader);

                var coresGrid = new UniformGrid { Columns = 4 };
                for (int i = 0; i < currentStats.CpuCoreUsages.Length; i++)
                {
                    var coreCard = CreateCoreUsageCard($"Core {i}", currentStats.CpuCoreUsages[i]);
                    coresGrid.Children.Add(coreCard);
                }
                panel.Children.Add(coresGrid);
            }

            content.Content = panel;
            MainContent.Content = content;
        }

        private Border CreateCoreUsageCard(string coreName, double usage)
        {
            var card = new Border
            {
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E1E1E")),
                CornerRadius = new CornerRadius(8),
                Margin = new Thickness(5),
                Padding = new Thickness(15)
            };

            var panel = new StackPanel();

            var nameBlock = new TextBlock
            {
                Text = coreName,
                FontSize = 12,
                Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#CCCCCC")),
                Margin = new Thickness(0, 0, 0, 5)
            };

            var usageBlock = new TextBlock
            {
                Text = $"{usage:F1}%",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = usage > 80 ? System.Windows.Media.Brushes.Red :
                           usage > 50 ? System.Windows.Media.Brushes.Orange :
                           new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#00FF88"))
            };

            panel.Children.Add(nameBlock);
            panel.Children.Add(usageBlock);

            card.Child = panel;
            return card;
        }

        private void LoadGpuContent()
        {
            var content = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Padding = new Thickness(20) };
            var panel = new StackPanel();

            var header = new TextBlock
            {
                Text = "GPU Performance Monitoring",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 0, 0, 20)
            };
            panel.Children.Add(header);

            var gpuCard = new Border
            {
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E1E1E")),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20)
            };

            var gpuPanel = new StackPanel();
            var gpuName = currentStats?.GpuInfo?.Name ?? "Loading...";
            var gpuUsage = currentStats?.GpuInfo?.Usage.ToString("F1") + "%" ?? "0%";
            var gpuTemp = currentStats?.GpuInfo?.Temperature.ToString("F1") + "°C" ?? "0°C";
            var vramUsage = currentStats?.GpuInfo != null ?
                $"{currentStats.GpuInfo.VramUsed:F1} GB / {currentStats.GpuInfo.VramTotal:F1} GB" : "Loading...";

            gpuPanel.Children.Add(new TextBlock { Text = "GPU Overview", FontSize = 16, FontWeight = FontWeights.SemiBold, Foreground = System.Windows.Media.Brushes.White, Margin = new Thickness(0, 0, 0, 15) });
            gpuPanel.Children.Add(CreateInfoItem("Graphics Card", gpuName));
            gpuPanel.Children.Add(CreateInfoItem("GPU Usage", gpuUsage));
            gpuPanel.Children.Add(CreateInfoItem("Temperature", gpuTemp));
            gpuPanel.Children.Add(CreateInfoItem("VRAM Usage", vramUsage));

            gpuCard.Child = gpuPanel;
            panel.Children.Add(gpuCard);

            content.Content = panel;
            MainContent.Content = content;
        }

        private void LoadMemoryContent()
        {
            var content = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Padding = new Thickness(20) };
            var panel = new StackPanel();

            var header = new TextBlock
            {
                Text = "Memory Usage Analysis",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 0, 0, 20)
            };
            panel.Children.Add(header);

            var memoryCard = new Border
            {
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E1E1E")),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20)
            };

            var memoryPanel = new StackPanel();
            var totalRam = currentStats?.MemoryInfo?.TotalGB.ToString("F1") + " GB" ?? "0 GB";
            var usedRam = currentStats?.MemoryInfo?.UsedGB.ToString("F1") + " GB" ?? "0 GB";
            var availableRam = currentStats?.MemoryInfo?.AvailableGB.ToString("F1") + " GB" ?? "0 GB";
            var usagePercent = currentStats?.MemoryInfo?.UsagePercentage.ToString("F1") + "%" ?? "0%";

            memoryPanel.Children.Add(new TextBlock { Text = "Memory Overview", FontSize = 16, FontWeight = FontWeights.SemiBold, Foreground = System.Windows.Media.Brushes.White, Margin = new Thickness(0, 0, 0, 15) });
            memoryPanel.Children.Add(CreateInfoItem("Total Memory", totalRam));
            memoryPanel.Children.Add(CreateInfoItem("Used Memory", usedRam));
            memoryPanel.Children.Add(CreateInfoItem("Available Memory", availableRam));
            memoryPanel.Children.Add(CreateInfoItem("Usage Percentage", usagePercent));

            memoryCard.Child = memoryPanel;
            panel.Children.Add(memoryCard);

            content.Content = panel;
            MainContent.Content = content;
        }

        private void LoadStorageContent()
        {
            var content = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Padding = new Thickness(20) };
            var panel = new StackPanel();

            var header = new TextBlock
            {
                Text = "Storage Analysis",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 0, 0, 20)
            };
            panel.Children.Add(header);

            if (currentStats?.StorageInfo != null)
            {
                foreach (var storage in currentStats.StorageInfo)
                {
                    var storageCard = new Border
                    {
                        Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E1E1E")),
                        CornerRadius = new CornerRadius(12),
                        Padding = new Thickness(20),
                        Margin = new Thickness(0, 0, 0, 15)
                    };

                    var storagePanel = new StackPanel();
                    storagePanel.Children.Add(new TextBlock { Text = $"Drive {storage.DriveLetter}", FontSize = 16, FontWeight = FontWeights.SemiBold, Foreground = System.Windows.Media.Brushes.White, Margin = new Thickness(0, 0, 0, 15) });
                    storagePanel.Children.Add(CreateInfoItem("Drive Type", storage.DriveType));
                    storagePanel.Children.Add(CreateInfoItem("Total Capacity", $"{storage.TotalGB:F1} GB"));
                    storagePanel.Children.Add(CreateInfoItem("Used Space", $"{storage.UsedGB:F1} GB"));
                    storagePanel.Children.Add(CreateInfoItem("Free Space", $"{storage.FreeGB:F1} GB"));
                    storagePanel.Children.Add(CreateInfoItem("Usage", $"{storage.UsagePercentage:F1}%"));
                    storagePanel.Children.Add(CreateInfoItem("Health Status", storage.Health));

                    storageCard.Child = storagePanel;
                    panel.Children.Add(storageCard);
                }
            }

            content.Content = panel;
            MainContent.Content = content;
        }

        private void LoadNetworkContent()
        {
            var content = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Padding = new Thickness(20) };
            var panel = new StackPanel();

            var header = new TextBlock
            {
                Text = "Network Monitor",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 0, 0, 20)
            };
            panel.Children.Add(header);

            var networkCard = new Border
            {
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E1E1E")),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20)
            };

            var networkPanel = new StackPanel();
            var downloadSpeed = currentStats?.NetworkInfo?.DownloadSpeed.ToString("F1") + " Mbps" ?? "0 Mbps";
            var uploadSpeed = currentStats?.NetworkInfo?.UploadSpeed.ToString("F1") + " Mbps" ?? "0 Mbps";
            var connectionStatus = currentStats?.NetworkInfo?.ConnectionStatus ?? "Unknown";

            networkPanel.Children.Add(new TextBlock { Text = "Network Overview", FontSize = 16, FontWeight = FontWeights.SemiBold, Foreground = System.Windows.Media.Brushes.White, Margin = new Thickness(0, 0, 0, 15) });
            networkPanel.Children.Add(CreateInfoItem("Connection Status", connectionStatus));
            networkPanel.Children.Add(CreateInfoItem("Download Speed", downloadSpeed));
            networkPanel.Children.Add(CreateInfoItem("Upload Speed", uploadSpeed));

            networkCard.Child = networkPanel;
            panel.Children.Add(networkCard);

            content.Content = panel;
            MainContent.Content = content;
        }

        private void LoadSecurityContent()
        {
            var content = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Padding = new Thickness(20) };
            var panel = new StackPanel();

            var header = new TextBlock
            {
                Text = "Security Center",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 0, 0, 20)
            };
            panel.Children.Add(header);

            var securityCard = new Border
            {
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E1E1E")),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20)
            };

            var securityPanel = new StackPanel();
            securityPanel.Children.Add(new TextBlock { Text = "Security Status", FontSize = 16, FontWeight = FontWeights.SemiBold, Foreground = System.Windows.Media.Brushes.White, Margin = new Thickness(0, 0, 0, 15) });
            securityPanel.Children.Add(CreateInfoItem("Windows Defender", "Active"));
            securityPanel.Children.Add(CreateInfoItem("Firewall", "Enabled"));
            securityPanel.Children.Add(CreateInfoItem("Last Scan", "2 hours ago"));
            securityPanel.Children.Add(CreateInfoItem("Threats Detected", "0"));

            securityCard.Child = securityPanel;
            panel.Children.Add(securityCard);

            content.Content = panel;
            MainContent.Content = content;
        }

        private void LoadProcessesContent()
        {
            var content = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Padding = new Thickness(20) };
            var panel = new StackPanel();

            var header = new TextBlock
            {
                Text = "Process Manager",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 0, 0, 20)
            };
            panel.Children.Add(header);

            var processCard = new Border
            {
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E1E1E")),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20)
            };

            var processPanel = new StackPanel();
            var totalProcesses = currentStats?.ProcessInfo?.TotalProcesses.ToString() ?? "0";
            var runningProcesses = currentStats?.ProcessInfo?.RunningProcesses.ToString() ?? "0";

            processPanel.Children.Add(new TextBlock { Text = "Process Overview", FontSize = 16, FontWeight = FontWeights.SemiBold, Foreground = System.Windows.Media.Brushes.White, Margin = new Thickness(0, 0, 0, 15) });
            processPanel.Children.Add(CreateInfoItem("Total Processes", totalProcesses));
            processPanel.Children.Add(CreateInfoItem("Running Processes", runningProcesses));

            processCard.Child = processPanel;
            panel.Children.Add(processCard);

            content.Content = panel;
            MainContent.Content = content;
        }

        private void LoadSettingsContent()
        {
            var content = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Padding = new Thickness(20) };
            var panel = new StackPanel();

            var header = new TextBlock
            {
                Text = "System Settings",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 0, 0, 20)
            };
            panel.Children.Add(header);

            var settingsCard = new Border
            {
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E1E1E")),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(20)
            };

            var settingsPanel = new StackPanel();
            settingsPanel.Children.Add(new TextBlock { Text = "Application Settings", FontSize = 16, FontWeight = FontWeights.SemiBold, Foreground = System.Windows.Media.Brushes.White, Margin = new Thickness(0, 0, 0, 15) });
            settingsPanel.Children.Add(CreateInfoItem("Update Frequency", "1 second"));
            settingsPanel.Children.Add(CreateInfoItem("Auto-start", "Enabled"));
            settingsPanel.Children.Add(CreateInfoItem("Theme", "Dark"));

            settingsCard.Child = settingsPanel;
            panel.Children.Add(settingsCard);

            content.Content = panel;
            MainContent.Content = content;
        }
    }
}