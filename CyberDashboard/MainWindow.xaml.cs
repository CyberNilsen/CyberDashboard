using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace CyberDashboard
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private Button currentActiveButton;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            SetActiveButton(DashboardBtn);
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            CurrentTime.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            timer?.Stop();
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
                    StatusText.Text = "All systems operational";
                    LoadDashboardContent();
                    break;
                case "CPU":
                    PageTitle.Text = "CPU Monitoring";
                    PageStatus.Text = "MONITORING";
                    StatusText.Text = "CPU metrics tracking";
                    LoadCpuContent();
                    break;
                case "GPU":
                    PageTitle.Text = "GPU Monitoring";
                    PageStatus.Text = "MONITORING";
                    StatusText.Text = "GPU metrics tracking";
                    LoadGpuContent();
                    break;
                case "Memory":
                    PageTitle.Text = "Memory Usage";
                    PageStatus.Text = "MONITORING";
                    StatusText.Text = "Memory usage tracking";
                    LoadMemoryContent();
                    break;
                case "Storage":
                    PageTitle.Text = "Storage Analysis";
                    PageStatus.Text = "MONITORING";
                    StatusText.Text = "Storage health monitoring";
                    LoadStorageContent();
                    break;
                case "Network":
                    PageTitle.Text = "Network Monitor";
                    PageStatus.Text = "MONITORING";
                    StatusText.Text = "Network traffic analysis";
                    LoadNetworkContent();
                    break;
                case "Security":
                    PageTitle.Text = "Security Center";
                    PageStatus.Text = "PROTECTED";
                    StatusText.Text = "System security active";
                    LoadSecurityContent();
                    break;
                case "Processes":
                    PageTitle.Text = "Process Manager";
                    PageStatus.Text = "MONITORING";
                    StatusText.Text = "Process monitoring active";
                    LoadProcessesContent();
                    break;
                case "Settings":
                    PageTitle.Text = "System Settings";
                    PageStatus.Text = "CONFIG";
                    StatusText.Text = "Configuration panel";
                    LoadSettingsContent();
                    break;
                default:
                    LoadDashboardContent();
                    break;
            }
        }

        private void LoadDashboardContent()
        {
           
        }

        private void LoadCpuContent()
        {
            var content = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Padding = new Thickness(20) };
            var panel = new StackPanel();

            var header = new TextBlock
            {
                Text = "CPU Performance Metrics",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 0, 0, 20)
            };
            panel.Children.Add(header);

            var grid = new UniformGrid { Columns = 2, Margin = new Thickness(0, 0, 0, 30) };

            var usageCard = CreateInfoCard("CPU Usage", "45%", "#00FF88", "\uE9D9");
            grid.Children.Add(usageCard);

            var tempCard = CreateInfoCard("Temperature", "67°C", "#FF6B35", "\uE7A6");
            grid.Children.Add(tempCard);

            panel.Children.Add(grid);

            var detailsCard = CreateDetailsCard("Processor Information", new[]
            {
                ("Model", "Intel Core i7-12700K"),
                ("Cores", "12 (8P + 4E)"),
                ("Threads", "20"),
                ("Base Clock", "3.6 GHz"),
                ("Boost Clock", "5.0 GHz"),
                ("Cache", "25 MB")
            });
            panel.Children.Add(detailsCard);

            content.Content = panel;
            MainContent.Content = content;
        }

        private void LoadGpuContent()
        {
            var content = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Padding = new Thickness(20) };
            var panel = new StackPanel();

            var header = new TextBlock
            {
                Text = "GPU Performance Metrics",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 0, 0, 20)
            };
            panel.Children.Add(header);

            var grid = new UniformGrid { Columns = 3, Margin = new Thickness(0, 0, 0, 30) };

            var usageCard = CreateInfoCard("GPU Usage", "67%", "#FF6B35", "\uE7F4");
            var tempCard = CreateInfoCard("Temperature", "72°C", "#FF6B35", "\uE7A6");
            var memCard = CreateInfoCard("VRAM Used", "8.2 GB", "#A855F7", "\uE950");

            grid.Children.Add(usageCard);
            grid.Children.Add(tempCard);
            grid.Children.Add(memCard);
            panel.Children.Add(grid);

            var detailsCard = CreateDetailsCard("Graphics Card Information", new[]
            {
                ("Model", "NVIDIA RTX 4080 Super"),
                ("VRAM", "16 GB GDDR6X"),
                ("Base Clock", "2295 MHz"),
                ("Boost Clock", "2550 MHz"),
                ("Memory Clock", "23 Gbps"),
                ("CUDA Cores", "10,240")
            });
            panel.Children.Add(detailsCard);

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

            var grid = new UniformGrid { Columns = 2, Margin = new Thickness(0, 0, 0, 30) };

            var usageCard = CreateInfoCard("RAM Usage", "12.4 GB / 32 GB", "#A855F7", "\uE950");
            var availableCard = CreateInfoCard("Available", "19.6 GB", "#00FF88", "\uE950");

            grid.Children.Add(usageCard);
            grid.Children.Add(availableCard);
            panel.Children.Add(grid);

            content.Content = panel;
            MainContent.Content = content;
        }

        private void LoadStorageContent()
        {
            var content = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Padding = new Thickness(20) };
            var panel = new StackPanel();

            var header = new TextBlock
            {
                Text = "Storage Drive Analysis",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 0, 0, 20)
            };
            panel.Children.Add(header);

            var grid = new UniformGrid { Columns = 2, Margin = new Thickness(0, 0, 0, 30) };

            var ssdCard = CreateInfoCard("SSD Usage", "856 GB / 2 TB", "#F59E0B", "\uEDA2");
            var healthCard = CreateInfoCard("Drive Health", "Excellent", "#00FF88", "\uE8AB");

            grid.Children.Add(ssdCard);
            grid.Children.Add(healthCard);
            panel.Children.Add(grid);

            content.Content = panel;
            MainContent.Content = content;
        }

        private void LoadNetworkContent()
        {
            var content = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Padding = new Thickness(20) };
            var panel = new StackPanel();

            var header = new TextBlock
            {
                Text = "Network Traffic Monitor",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 0, 0, 20)
            };
            panel.Children.Add(header);

            var grid = new UniformGrid { Columns = 2, Margin = new Thickness(0, 0, 0, 30) };

            var downloadCard = CreateInfoCard("Download", "125 Mbps", "#00D4FF", "\uE896");
            var uploadCard = CreateInfoCard("Upload", "45 Mbps", "#00FF88", "\uE898");

            grid.Children.Add(downloadCard);
            grid.Children.Add(uploadCard);
            panel.Children.Add(grid);

            content.Content = panel;
            MainContent.Content = content;
        }

        private void LoadSecurityContent()
        {
            var content = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Padding = new Thickness(20) };
            var panel = new StackPanel();

            var header = new TextBlock
            {
                Text = "Security Status Center",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 0, 0, 20)
            };
            panel.Children.Add(header);

            var statusCard = CreateInfoCard("Security Status", "Protected", "#00FF88", "\uE72E");
            panel.Children.Add(statusCard);

            content.Content = panel;
            MainContent.Content = content;
        }

        private void LoadProcessesContent()
        {
            var content = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Padding = new Thickness(20) };
            var panel = new StackPanel();

            var header = new TextBlock
            {
                Text = "Active Processes",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 0, 0, 20)
            };
            panel.Children.Add(header);

            var processCard = CreateInfoCard("Active Processes", "247 Running", "#00D4FF", "\uE9D5");
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
                Text = "Dashboard Settings",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 0, 0, 20)
            };
            panel.Children.Add(header);

            var settingsCard = CreateInfoCard("Configuration", "Ready", "#A855F7", "\uE713");
            panel.Children.Add(settingsCard);

            content.Content = panel;
            MainContent.Content = content;
        }

        private Border CreateInfoCard(string title, string value, string color, string icon)
        {
            var card = new Border
            {
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E1E1E")),
                CornerRadius = new CornerRadius(12),
                Margin = new Thickness(5),
                Padding = new Thickness(20)
            };

            var panel = new StackPanel();

            var headerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var iconText = new TextBlock
            {
                Text = icon,
                FontFamily = new System.Windows.Media.FontFamily("Segoe MDL2 Assets"),
                FontSize = 20,
                Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color))
            };

            var titleText = new TextBlock
            {
                Text = title,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(10, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            headerPanel.Children.Add(iconText);
            headerPanel.Children.Add(titleText);

            var valueText = new TextBlock
            {
                Text = value,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color))
            };

            panel.Children.Add(headerPanel);
            panel.Children.Add(valueText);

            card.Child = panel;
            return card;
        }

        private Border CreateDetailsCard(string title, (string key, string value)[] details)
        {
            var card = new Border
            {
                Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E1E1E")),
                CornerRadius = new CornerRadius(12),
                Margin = new Thickness(0, 20, 0, 0),
                Padding = new Thickness(20)
            };

            var panel = new StackPanel();

            var titleText = new TextBlock
            {
                Text = title,
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(0, 0, 0, 15)
            };
            panel.Children.Add(titleText);

            foreach (var (key, value) in details)
            {
                var detailPanel = new StackPanel { Margin = new Thickness(0, 5, 0, 5) };

                var keyText = new TextBlock
                {
                    Text = key,
                    FontSize = 10,
                    Foreground = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#999999"))
                };

                var valueText = new TextBlock
                {
                    Text = value,
                    FontSize = 12,
                    Foreground = System.Windows.Media.Brushes.White
                };

                detailPanel.Children.Add(keyText);
                detailPanel.Children.Add(valueText);
                panel.Children.Add(detailPanel);
            }

            card.Child = panel;
            return card;
        }
    }
}