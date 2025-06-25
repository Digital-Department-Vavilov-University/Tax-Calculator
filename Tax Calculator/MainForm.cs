using System;
using System.Data;
using System.Windows.Forms;

namespace TaxCalculatorApp
{
    public partial class MainForm : Form
    {
        private TextBox txtIncome;
        private ComboBox cmbTaxType;
        private Label lblResult;
        private DataGridView dgvHistory;

        // Словарь для хранения истории по типам налогов
        private readonly System.Collections.Generic.Dictionary<string, DataTable> taxHistories =
            new System.Collections.Generic.Dictionary<string, DataTable>();

        private TaxCalculator calculator = new TaxCalculator();

        public MainForm()
        {
            InitializeComponent();
            InitializeHistoryTables();
        }

        private void InitializeHistoryTables()
        {
            string[] taxTypes = {
                "НДФЛ 13%",
                "НДФЛ 30%",
                "Имущественный вычет (продажа недвижимости)"
            };

            foreach (var type in taxTypes)
            {
                DataTable table = new DataTable(type);
                table.Columns.Add("Дата", typeof(DateTime));
                table.Columns.Add("Входное значение", typeof(double));
                table.Columns.Add("Результат", typeof(double));
                taxHistories[type] = table;
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Налоговый калькулятор РФ";
            this.Width = 600;
            this.Height = 500;

            // === Значение ===
            Label lblIncome = new Label();
            lblIncome.Text = "Значение:";
            lblIncome.Location = new System.Drawing.Point(20, 20);

            txtIncome = new TextBox();
            txtIncome.Location = new System.Drawing.Point(150, 20);
            txtIncome.Width = 150;

            // === Тип налога ===
            Label lblTaxType = new Label();
            lblTaxType.Text = "Тип налога:";
            lblTaxType.Location = new System.Drawing.Point(20, 60);

            cmbTaxType = new ComboBox();
            cmbTaxType.Location = new System.Drawing.Point(150, 60);
            cmbTaxType.Width = 250;
            cmbTaxType.Items.AddRange(new object[]
            {
                "НДФЛ 13%",
                "НДФЛ 30%",
                "Имущественный вычет (продажа недвижимости)"
            });
            cmbTaxType.SelectedIndex = 0;

            // === Кнопка рассчитать ===
            Button btnCalculate = new Button();
            btnCalculate.Text = "Рассчитать";
            btnCalculate.Location = new System.Drawing.Point(150, 100);
            btnCalculate.Width = 100;

            // === Кнопка очистить ===
            Button btnClear = new Button();
            btnClear.Text = "Очистить";
            btnClear.Location = new System.Drawing.Point(260, 100);
            btnClear.Width = 100;

            // === Результат ===
            lblResult = new Label();
            lblResult.Location = new System.Drawing.Point(20, 140);
            lblResult.Width = 300;
            lblResult.ForeColor = System.Drawing.Color.Blue;

            // === Таблица истории ===
            dgvHistory = new DataGridView();
            dgvHistory.Location = new System.Drawing.Point(20, 180);
            dgvHistory.Width = 540;
            dgvHistory.Height = 250;
            dgvHistory.AutoGenerateColumns = true;
            dgvHistory.ReadOnly = true;

            // === Обработчики событий ===
            cmbTaxType.SelectedIndexChanged += CmbTaxType_SelectedIndexChanged;
            btnCalculate.Click += BtnCalculate_Click;
            btnClear.Click += BtnClear_Click;

            // === Добавление элементов на форму ===
            this.Controls.Add(lblIncome);
            this.Controls.Add(txtIncome);
            this.Controls.Add(lblTaxType);
            this.Controls.Add(cmbTaxType);
            this.Controls.Add(btnCalculate);
            this.Controls.Add(btnClear);
            this.Controls.Add(lblResult);
            this.Controls.Add(dgvHistory);

            // === Загрузить историю текущего типа ===
            LoadHistoryToGrid();
        }

        private void CmbTaxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadHistoryToGrid();
        }

        private void LoadHistoryToGrid()
        {
            string selectedType = cmbTaxType.SelectedItem.ToString();
            if (taxHistories.ContainsKey(selectedType))
            {
                dgvHistory.DataSource = new BindingSource(taxHistories[selectedType], null);
            }
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(txtIncome.Text, out double value))
            {
                MessageBox.Show("Введите корректное числовое значение.");
                return;
            }

            string selectedTax = cmbTaxType.SelectedItem.ToString();

            double tax = 0;

            switch (selectedTax)
            {
                case "НДФЛ 13%":
                    tax = calculator.CalculateNDFl(value, 13);
                    break;
                case "НДФЛ 30%":
                    tax = calculator.CalculateNDFl(value, 30);
                    break;
                case "Имущественный вычет (продажа недвижимости)":
                    tax = calculator.CalculatePropertyTax(value);
                    break;
            }

            lblResult.Text = $"Сумма налога: {tax:F2} ₽";

            // Добавляем запись в историю
            DataRow row = taxHistories[selectedTax].NewRow();
            row["Дата"] = DateTime.Now;
            row["Входное значение"] = value;
            row["Результат"] = tax;
            taxHistories[selectedTax].Rows.Add(row);

            LoadHistoryToGrid();
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtIncome.Clear();
            lblResult.Text = string.Empty;
            txtIncome.Focus();

            string selectedTax = cmbTaxType.SelectedItem.ToString();
            if (taxHistories.ContainsKey(selectedTax))
            {
                taxHistories[selectedTax].Rows.Clear();
                LoadHistoryToGrid();
            }
        }
    }
}