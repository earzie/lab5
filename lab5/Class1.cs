using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.CodeDom.Compiler;

namespace lab5
{
    class Depart
    {
        private int _AoEmp;     //Число працівників
        private int _AoRes;     //Кількість ресурсів, що витрачається
        private float _AoChng;  //Кількість товарів
        public Depart()
        {
            _AoEmp = 0;
            _AoRes = 0;
            _AoChng = 0;
        }
        public Depart(int aoEmp, int aoRes, float aoChng)
        {
            _AoEmp = aoEmp;
            _AoRes = aoRes;
            _AoChng = aoChng;
        }
        public void EmpPlus() 
        {
            _AoEmp += 1;
        }
        public void EmpMinus()
        {
            _AoEmp -= 1;
        }
        public int GetAoEmp()
        {
            return _AoEmp;
        }
        public int GetAoRes() 
        {
            return _AoRes;
        }
        public float GetAoChng()
        {
            return _AoChng;
        }
    }
    internal class Factory
    {
        private List<Depart> _depart = new();   //Список цехів
        private double _money;                  //Грошей на банківському рахунку
        private int _AoD;                       //Кількість цехів
        private int _warehouse;                 //Кількість ресурсів на складі
        private float _Done;                    //Кількість зроблених товарів
        private float _Cost;                    //Ціна закупки ресурсів
        private bool _started = false;           //Чи працює зараз завод
        private String statsh = "Idle";
        private String moneysh;
        private String whsh;
        public Factory()
        {
            _money = 0.0;
            _AoD = 0;
        }
        public Factory(List<Depart> depart, double money, int warehouse, float cost)
        {
            _depart = depart;
            _money = money;
            _AoD = _depart.Count;
            _warehouse = warehouse;
            _Done = 0;
            _Cost = cost;
        }
        public bool Run()
        {
            if (_depart.Count != 0)
                statsh = "Status: Running";
                for (int i = 0; i < _depart.Count; i++)
                {
                    if (_warehouse != 0 && _warehouse - _depart[i].GetAoRes() >= 0)
                    {
                        _warehouse -= _depart[i].GetAoRes();
                        _Done += _depart[i].GetAoChng();

                    }
                    else
                    {
                        if (WarehouseNSalary(statsh))
                        return true;
                    else
                        {
                            return false;
                        }
                    }
                }
                Thread.Sleep(1000);
                return true;
        }
        public void Sell()
        {
            _money += _Cost * 1.3 * _Done;
            _Done = 0;
            statsh = "Status: Selling products";
        }
        public bool WarehouseNSalary(String stat)
        {
            if (_money < 0)
            {
                statsh = "Status: Bankrupt";
                return false;
            }
            else
                statsh = "Status: Purchasing resources";
            if (_depart.Count != 0)
            {
                int temp = 0;
                for (int i = 0; i < _depart.Count; i++)
                {
                    temp += _depart[i].GetAoRes();
                    _money -= _Cost * 0.15 * _depart[i].GetAoEmp();
                }
                _money -= temp * _Cost;
                _warehouse += temp;
            }
            return true;
        }
        public void SetStatus(bool started)
        {
            _started = started;
        }
        public bool GetStatus()
        {
            return _started;
        }
        public double GetMoney() { return _money; }
        public double GetWarehouse() { return _warehouse; }
        public void GetLabels(Label stat, Label money, Label wh) {
            moneysh = String.Format("Money: {0}", _money);
            whsh = String.Format("Warehouse: {0}", _warehouse);
            stat.Invoke((MethodInvoker)(() => stat.Text = statsh));
            money.Invoke((MethodInvoker)(() => money.Text = moneysh));
            wh.Invoke((MethodInvoker)(() => wh.Text = whsh));
        }
        public string GetStatString()
        {
            return statsh;
        }
    }
}
