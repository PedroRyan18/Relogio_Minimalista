using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms; // Biblioteca para NotifyIcon
using System.Windows.Threading;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private NotifyIcon _notifyIcon; // Ícone na bandeja do sistema
        private string textoData = "Esconder Data";

        public MainWindow()
        {
            InitializeComponent();

            // Configuração do relógio para atualizar a cada segundo
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Tick += Date_Tick;
            timer.Start();

            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;

            var windowWidth = this.Width;
            var windowHeight = this.Height;

            this.Left = screenWidth - windowWidth - 10; // 10px de margem
            this.Top = screenHeight - windowHeight - 40;

            // Configurar NotifyIcon (ícone da bandeja)
            _notifyIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon("clock-with-white-face_icon-icons.com_72804.ico"),
                Visible = true,
                Text = "Clock App"
            };

            // Menu de contexto do NotifyIcon
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Abrir", null, (s, e) => ShowWindow());
            contextMenu.Items.Add("Minimizar", null, (s, e) => HideWindow());
            contextMenu.Items.Add(textoData, null, (s, e) => EsconderData());
            contextMenu.Items.Add("Alterar Cor", null, (s, e) => AlterarCorComColorPicker()); // Abre o Color Picker
            contextMenu.Items.Add("Fechar", null, (s, e) => CloseApp());
            _notifyIcon.ContextMenuStrip = contextMenu;

            // Evento de clique no ícone da bandeja
            _notifyIcon.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                    ShowWindow();
            };
        }

        private void EsconderData()
        {
            // Localiza o item do menu de contexto correspondente
            var item = _notifyIcon.ContextMenuStrip.Items[2] as ToolStripMenuItem;

            if (textoData == "Esconder Data")
            {
                DateText.Visibility = Visibility.Collapsed;
                textoData = "Mostrar Data";
                TelaPrincipal.Height = 45;
            }
            else
            {
                DateText.Visibility = Visibility.Visible;
                textoData = "Esconder Data";
                TelaPrincipal.Height = 60;
            }

            // Atualiza o texto do item do menu de contexto
            if (item != null)
            {
                item.Text = textoData;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ClockText.Text = DateTime.Now.ToString("HH:mm"); // Atualiza o texto do relógio
        }

        private void Date_Tick(object sender, EventArgs e)
        {
            DateText.Text = DateTime.Now.ToString("dd/MM/yyyy"); // Atualiza o texto da data
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Permite arrastar a janela clicando nela
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void ShowWindow()
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
        }

        private void HideWindow()
        {
            this.Hide();
        }

        private void CloseApp()
        {
            _notifyIcon.Dispose();
            System.Windows.Application.Current.Shutdown();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            _notifyIcon.Dispose(); // Limpa o ícone da bandeja ao fechar o aplicativo
        }

        private void AlterarCorComColorPicker()
        {
            // Usar o ColorDialog para abrir o seletor de cores
            using (var colorDialog = new System.Windows.Forms.ColorDialog())
            {
                // Mostrar o diálogo ao usuário
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Obter a cor selecionada
                    var corSelecionada = colorDialog.Color;

                    // Converter System.Drawing.Color para System.Windows.Media.Color
                    var mediaColor = System.Windows.Media.Color.FromArgb(corSelecionada.A, corSelecionada.R, corSelecionada.G, corSelecionada.B);

                    // Aplicar a cor ao ClockText e ao DateText
                    ClockText.Foreground = new System.Windows.Media.SolidColorBrush(mediaColor);
                    DateText.Foreground = new System.Windows.Media.SolidColorBrush(mediaColor);
                }
            }
        }

    }
}
