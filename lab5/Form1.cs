using static System.Windows.Forms.AxHost;

namespace lab5
{
    public partial class Form1 : Form
    {
        private List<Factory> factories = new();
        private List<Depart> departs = new();
        private static int Len = 0;
        private int page = 0;
        private Thread Runned;
        private Thread Seller;
        private bool started = false;
        AutoResetEvent evt = new AutoResetEvent(true);
        AutoResetEvent evts = new AutoResetEvent(true);
        StreamWriter sw;

        public void Updater()
        {
            if (Len > 0)
            {
                label1.Invoke((MethodInvoker)(() => label1.Text = "ID: " + Convert.ToString(page)));
                factories[page].GetLabels(label4, label2, label3);
            }
        }
        public Form1()
        {
            InitializeComponent();
            Runned = new Thread(
            () =>
            {
                while (started)
                {
                    evt.WaitOne();
                    if (Len > 0)
                    {
                        for (int i = 0; i < Len; i++)
                        {
                            factories[i].Run();
                            Updater();
                            if (started) sw.WriteLine(String.Format("[RUN] ID: {0}, Money: {1}, Warehouse: {2}, Status: {3}", i, factories[i].GetMoney(), factories[i].GetWarehouse(), factories[i].GetStatString()));
                        }
                    }

                    if (started) evt.Set();
                }
            }
            );
            Seller = new Thread(
            () =>
            {
                while (started)
                {
                    evts.WaitOne();
                    if (Len > 0)
                    {
                        for (int i = 0; i < Len; i++)
                        {
                            factories[i].Sell();
                            Updater();
                            if(started) sw.WriteLine(String.Format("[SELL] ID: {0}, Money: {1}, Warehouse: {2}, Status: {3}", i, factories[i].GetMoney(), factories[i].GetWarehouse(), factories[i].GetStatString()));
                        }
                    }
                    Thread.Sleep(3000);
                    if (started) evts.Set();
                }
            }
            );
            Runned.IsBackground = true;
            Seller.IsBackground = true;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            departs.Add(new Depart(65, 28, 137));
            departs.Add(new Depart(57, 20, 137));
            departs.Add(new Depart(88, 42, 137));
            departs.Add(new Depart(120, 50, 137));
            departs.Add(new Depart(34, 30, 137));
            departs.Add(new Depart(20, 10, 137));

            factories.Add(new Factory(departs, 2300000, 31000, 70));
            factories.Add(new Factory(departs, 200000, 1000, 50));
            factories.Add(new Factory(departs, 100000, 3000, 103));
            Len = factories.Count;
            
            label1.Text = "ID: 0";
            label2.Text = String.Format("Money: {0}", factories[0].GetMoney());
            label3.Text = String.Format("Warehouse: {0}", factories[0].GetWarehouse());
            label4.Text = "Press Start to start";
            button3.Enabled = true;
            button4.Enabled = false;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (page != 0)
            {
                page--;
            }
            Updater();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(page < Len - 1)
            {
                page++;
            }
            Updater();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            started = true;
            sw = new StreamWriter("D:\\Test.xml");
            for (int i = 0; i < Len; i++)
                factories[i].SetStatus(started);
            if (!Runned.IsAlive)
            {
                Runned.Start();
                Seller.Start();
            }
            button4.Enabled = true;
            button3.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            started = false;
            for (int i = 0; i < factories.Count; i++)
            {
                factories[i].SetStatus(false);
            }
            button3.Enabled = true;
            button4.Enabled = false;
            sw.Close();
        }
    }
}