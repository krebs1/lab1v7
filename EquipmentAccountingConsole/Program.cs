using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EquipmentAccountingConsole
{
    public abstract class Equipment
    {
        public int Id = 0;
        public string Name { get; }
        public string SerialNumber { get; }
        public string RegistrationDate { get; }
        public string LastMaintenanceDate { get; }
        
        public Equipment(string name, string serialNumber, string registrationDate, string lastMaintenanceDate)
        {
            Name = name;
            SerialNumber = serialNumber;
            RegistrationDate = registrationDate;
            LastMaintenanceDate = lastMaintenanceDate;
        }
    }

    class DecomEquipmentByTime : Equipment
    {
        public string DecomissedDate { get; }

        public DecomEquipmentByTime(string name, string serialNumber, string registrationDate, string lastMaintenanceDate, string decomissedDate) : base(name, serialNumber, registrationDate, lastMaintenanceDate)
        {
            DecomissedDate = decomissedDate;
        }
    }

    class DecomEquipmentByReason : Equipment
    {
        private string DecomissedReason;
        public string DecomissedDate { get; }

        public DecomEquipmentByReason(string name, string serialNumber, string registrationDate, string lastMaintenanceDate, string decomissedReason, string decomissedDate) : base(name, serialNumber, registrationDate, lastMaintenanceDate)
        {
            DecomissedDate = decomissedDate;
            DecomissedReason = decomissedReason;
        }
    }

    public interface IDataSource
    {
        Equipment Save(Equipment record);
        Equipment Get(int id);
        bool Delete(int id);
        List<Equipment> GetAll();
    }

    class MemoryDataSource : IDataSource
    {
        private List<Equipment> records = new List<Equipment>();
        
        public Equipment Save(Equipment record)
        {
            if (record.Id == 0)
            {
                record.Id = records.Count + 1;
                records.Add(record);
            }
            else
            {
                records[record.Id - 1] = record;
            }
            
            return records[record.Id - 1];
        }

        public Equipment Get(int id)
        {
            return records.Find(item => item.Id == id);
        }

        public bool Delete(int id)
        {
            return records.Remove(records.Find(item => item.Id == id));
        }

        public List<Equipment> GetAll()
        {
            return records;
        }
    }

    class BusinessLogic
    {
        private IDataSource _dataSource;
        private string _datePattern = @"\d\d\d\d/\d\d/\d\d", _serialNumberPattern = @"\d\d-\d\d\d";

        public BusinessLogic(IDataSource dataSource)
        {
            _dataSource = dataSource;
        }
        
        public Equipment SaveRecord(Equipment record)
        {
            if (Regex.Match(record.SerialNumber, _serialNumberPattern).Success &&
                Regex.Match(record.RegistrationDate, _datePattern).Success &&
                Regex.Match(record.LastMaintenanceDate, _datePattern).Success)

            {
                return _dataSource.Save(record);
            }
            else
            {
                throw new Exception("Неверный ввод данных, повторите ввод (дата в формате: ГГГГ/ММ/ДД, серийный номер в формате: 99-999)");
            }
        }

        public Equipment UpdateRecord(Equipment record)
        {
            if (Regex.Match(record.SerialNumber, _serialNumberPattern).Success &&
                Regex.Match(record.RegistrationDate, _datePattern).Success &&
                Regex.Match(record.LastMaintenanceDate, _datePattern).Success)
            {
                return _dataSource.Save(record);
            }
            else
            {
                throw new Exception("Неверный ввод данных, повторите ввод (дата в формате: ГГГГ/ММ/ДД, серийный номер в формате: 99-999)");
            }
        }

        public bool DeleteRecord(int id)
        {
            return _dataSource.Delete(id);
        }

        public List<Equipment> GetAllRecords()
        {
            return _dataSource.GetAll().OrderBy(x => x.Name).ThenBy(x => x.SerialNumber).ToList();
        }

        public Equipment GetRecord(int id)
        {
            return _dataSource.Get(id);
        }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var businessLogic = new BusinessLogic(new MemoryDataSource());

            string[] MenuItems = { "Добавить запись", "Посмотреть записи", "Изменить запись", "Удалить запись", "Выйти" };
            string[] RecordTypes = { "Оборудование списанное по времени", "Оборудование списанное по другой причине" };
            int MenuItemNumber = 0;

            bool isWork = true;
            while (isWork)
            {
                Console.Clear();
                
                Console.WriteLine("Выберите действие (стрелки вверх и вниз, Enter)");
                for (int i = 0; i < MenuItems.Length; i++)
                {
                    if (MenuItemNumber == i)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("> ");
                        Console.WriteLine(MenuItems[i]);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.Write("> ");
                        Console.WriteLine(MenuItems[i]);
                    }
                }
                
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (MenuItemNumber == 0) MenuItemNumber = MenuItems.Length - 1;
                        else MenuItemNumber--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (MenuItemNumber == MenuItems.Length - 1) MenuItemNumber = 0;
                        else MenuItemNumber++;
                        break;
                    case ConsoleKey.Enter:
                        switch (MenuItems[MenuItemNumber])
                        {
                            case "Добавить запись":
                                MenuItemNumber = 0;
                                bool SaveDone = false;
                                
                                while (!SaveDone)
                                {
                                    Console.Clear();
                                    Console.WriteLine("Выберите вид записи (стрелки вверх и вниз, Enter)");
                                    
                                    for (int i = 0; i < RecordTypes.Length; i++)
                                    {
                                        if (MenuItemNumber == i)
                                        {
                                            Console.ForegroundColor = ConsoleColor.Red;
                                            Console.Write("> ");
                                            Console.WriteLine(RecordTypes[i]);
                                            Console.ForegroundColor = ConsoleColor.White;
                                        }
                                        else
                                        {
                                            Console.Write("> ");
                                            Console.WriteLine(RecordTypes[i]);
                                        }
                                    }
                                    
                                    key = Console.ReadKey(true).Key;
                                    switch (key)
                                    {
                                        case ConsoleKey.UpArrow:
                                            if (MenuItemNumber == 0) MenuItemNumber = RecordTypes.Length - 1;
                                            else MenuItemNumber--;
                                            break;
                                        case ConsoleKey.DownArrow:
                                            if (MenuItemNumber == RecordTypes.Length - 1) MenuItemNumber = 0;
                                            else MenuItemNumber++;
                                            break;
                                        case ConsoleKey.Enter:
                                            switch (RecordTypes[MenuItemNumber])
                                            {
                                                case "Оборудование списанное по времени":
                                                    string mes = null;
                                                    
                                                    while (true)
                                                    {
                                                        Console.Clear();
                                                        if (mes != null) Console.WriteLine(mes);
                                                        Console.WriteLine("Название");
                                                        Console.Write("> ");
                                                        var name = Console.ReadLine();
                                                        Console.WriteLine("Серийный номер");
                                                        Console.Write("> ");
                                                        var serialNumber = Console.ReadLine();
                                                        Console.WriteLine("Дата регистрации");
                                                        Console.Write("> ");
                                                        var regDate = Console.ReadLine();
                                                        Console.WriteLine("Дата последнего обслуживания");
                                                        Console.Write("> ");
                                                        var lastMaintenance = Console.ReadLine();
                                                        Console.WriteLine("Дата списания");
                                                        Console.Write("> ");
                                                        var decomDate = Console.ReadLine();

                                                        var record = new DecomEquipmentByTime(name, serialNumber,
                                                            regDate, lastMaintenance, decomDate);

                                                        try
                                                        {
                                                            businessLogic.SaveRecord(record);
                                                            SaveDone = true;
                                                            MenuItemNumber = 0;
                                                            break;
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            mes = e.Message;
                                                        }
                                                    }
                                                    break;
                                                case "Оборудование списанное по другой причине":
                                                    string mess = null;
                                                    
                                                    while (true)
                                                    {
                                                        Console.Clear();
                                                        if (mess != null) Console.WriteLine(mess);
                                                        Console.WriteLine("Название");
                                                        Console.Write("> ");
                                                        var name = Console.ReadLine();
                                                        Console.WriteLine("Серийный номер");
                                                        Console.Write("> ");
                                                        var serialNumber = Console.ReadLine();
                                                        Console.WriteLine("Дата регистрации");
                                                        Console.Write("> ");
                                                        var regDate = Console.ReadLine();
                                                        Console.WriteLine("Дата последнего обслуживания");
                                                        Console.Write("> ");
                                                        var lastMaintenance = Console.ReadLine();
                                                        Console.WriteLine("Дата списания");
                                                        Console.Write("> ");
                                                        var decomDate = Console.ReadLine();
                                                        Console.WriteLine("Причина списания");
                                                        Console.Write("> ");
                                                        var decomReason = Console.ReadLine();

                                                        var record = new DecomEquipmentByReason(name, serialNumber,
                                                            regDate, lastMaintenance, decomDate, decomReason);

                                                        try
                                                        {
                                                            businessLogic.SaveRecord(record);
                                                            SaveDone = true;
                                                            MenuItemNumber = 0;
                                                            break;
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            mess = e.Message;
                                                        }
                                                    }
                                                    break;
                                            }
                                            break;
                                    }
                                }
                                break;
                            case "Посмотреть записи":
                                Console.Clear();
                                var records = businessLogic.GetAllRecords();
                                Console.WriteLine($"Всего записей: {records.Count} (нажмите Enter для выхода)");
                                foreach (var record in records)
                                {
                                    var recordClass = TypeDescriptor.GetClassName(record).Split('.').Last();
                                    Console.Write($"id: {record.Id}   | ");
                                    switch (recordClass)
                                    {
                                        case "DecomEquipmentByReason":
                                            Console.Write($"Оборудование списаное по другой причине | ");
                                            break;
                                        case "DecomEquipmentByTime":
                                            Console.Write($"Оборудование списаное по времени        | ");
                                            break;
                                    }
                                    Console.Write($"{record.Name   } | ");
                                    Console.Write($"{record.SerialNumber} | ");
                                    Console.Write($"{record.RegistrationDate} | ");
                                    Console.Write($"{record.LastMaintenanceDate} |");
                                    Console.WriteLine();
                                }

                                Console.ReadLine();
                                break;
                            case "Изменить запись":
                                bool updateDone = false;
                                string mes2 = null;
                                while (!updateDone)
                                {
                                    Console.Clear();
                                    Console.WriteLine("Введите идентификатор записи, для выхода введите 0");
                                    if (mes2 != null) Console.WriteLine(mes2);
                                    Console.Write("> ");
                                    var id = Convert.ToInt32(Console.ReadLine());
                                    
                                    if(id == 0) break;

                                    var record = businessLogic.GetRecord(id);
                                    var recordClass = TypeDescriptor.GetClassName(record).Split('.').Last();
                                    if (record == null) mes2 = "Неверный идентификатор";
                                    else
                                    {
                                        mes2 = null;
                                        while (!updateDone)
                                        {
                                            Console.Clear();
                                            if (mes2 != null) Console.WriteLine(mes2);
                                            Console.WriteLine("Название");
                                            Console.Write("> ");
                                            var name = Console.ReadLine();
                                            Console.WriteLine("Серийный номер");
                                            Console.Write("> ");
                                            var serialNumber = Console.ReadLine();
                                            Console.WriteLine("Дата регистрации");
                                            Console.Write("> ");
                                            var regDate = Console.ReadLine();
                                            Console.WriteLine("Дата последнего обслуживания");
                                            Console.Write("> ");
                                            var lastMaintenance = Console.ReadLine();
                                            Console.WriteLine("Дата списания");
                                            Console.Write("> ");
                                            var decomDate = Console.ReadLine();
                                            switch (recordClass)
                                            {
                                                case "DecomEquipmentByReason":
                                                    Console.WriteLine("Причина списания");
                                                    Console.Write("> ");
                                                    var decomReason = Console.ReadLine();

                                                    var updatedRecBR = new DecomEquipmentByReason(name, serialNumber, regDate, lastMaintenance, decomDate, decomReason);
                                                    updatedRecBR.Id = record.Id;

                                                    try
                                                    {
                                                        businessLogic.UpdateRecord(updatedRecBR);
                                                        updateDone = true;
                                                    }
                                                    catch(Exception e)
                                                    {
                                                        mes2 = e.Message;
                                                    }
                                                    break;
                                                case "DecomEquipmentByTime":
                                                    var updatedRecBT = new DecomEquipmentByTime(name, serialNumber, regDate, lastMaintenance, decomDate);
                                                    updatedRecBT.Id = record.Id;
                                                    
                                                    try
                                                    {
                                                        businessLogic.UpdateRecord(updatedRecBT);
                                                        updateDone = true;
                                                    }
                                                    catch(Exception e)
                                                    {
                                                        mes2 = e.Message;
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                }
                                break;
                            case "Удалить запись":
                                string mes3 = null;
                                bool delDone = false;
                                while (!delDone)
                                {
                                    Console.Clear();
                                    Console.WriteLine("Введите идентификатор записи, для выхода введите 0");
                                    if (mes3 != null) Console.WriteLine(mes3);
                                    Console.Write("> ");
                                    var idDel = Convert.ToInt32(Console.ReadLine());
                                    
                                    if(idDel == 0) break;
                                    Console.Clear();
                                    Console.WriteLine($"Хотите удалить запись с id: {idDel}? (1 - да, 0 - нет)");
                                    Console.Write("> ");
                                    switch (Console.ReadLine())
                                    {
                                        case "0":
                                            delDone = true;
                                            break;
                                        case "1":
                                            if (businessLogic.DeleteRecord(idDel))
                                            {
                                                Console.Clear();
                                                Console.WriteLine("Запись успешно удалена. Нажмите Enter для выхода");
                                                Console.ReadLine();
                                                delDone = true;
                                            }
                                            else
                                            {
                                                mes3 = "Неверный идентификатор";
                                            }
                                            break;
                                    }
                                }
                                break;
                            case "Выйти":
                                isWork = false;
                                Console.Clear();
                                Console.WriteLine("Выход");
                                break;
                        }
                        break;
                }
            }
        }
    }
}