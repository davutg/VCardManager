using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VCF_VCardManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ContactVM vm = new ContactVM();
        public MainWindow()
        {
            this.Resources.Add("vm", vm);
            this.DataContext = vm;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.ShowDialog(this);
            if (String.IsNullOrEmpty(fd.FileName))
                return;
            var lines = File.ReadLines(fd.FileName);
            String s = File.ReadAllText(fd.FileName);
            vm.FileName = fd.FileName;
            Thread t = new Thread(new ParameterizedThreadStart((p) =>
            {
                Regex r = new Regex(@"BEGI.+?CARD(.+?)EN.+?CARD", RegexOptions.Singleline);
                Regex n = new Regex("N*?:(.+?);{2,}");
                Regex tel = new Regex("TEL(.+?):(.+?)[\r\n|\r|\n]");
                int i = 0;
                var matches = r.Matches((string)p);
                foreach (Match match in matches)
                {
                    String data = match.Groups[1].ToString();
                    Debug.WriteLine(match.Groups[1]);
                    var name = n.Match(data);
                    String contactName = name.Groups[1].ToString();
                    if (contactName.StartsWith(";"))
                    {
                        contactName = contactName.Substring(1);
                    }
                    else if (contactName.Contains(";"))
                    {
                        int ndx = contactName.IndexOf(";");
                        contactName = contactName.Substring(ndx+1, contactName.Length-1 - ndx) +" "+ contactName.Substring(0,ndx);
                    }
                    contactName = contactName.Replace(";", " ");
                    MatchCollection phones = tel.Matches(data);
                    String phone = "";
                    foreach (Match p1 in phones)
                    {
                        phone = phone + (phone.Length == 0 ? "" : Environment.NewLine) + p1.Groups[2];
                    }
                    phone = phone.Replace("-", "");
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (!String.IsNullOrEmpty(phone))
                        {
                            
                            var con = new Contact() { ContactName = contactName, Phone = phone, ItemNo = i,IsRequired=true };
                            if (!vm.Contacts.Where(w => w.ContactName == contactName && w.Phone == phone).Any()) //duplicate
                            {
                                vm.Contacts.Add(con);
                                i++;
                            }
                        }
                    });
                }
                String result = String.Format("Reading over: {0} records found", i);
                Debug.WriteLine(result);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    vm.Status = result;
                });
            }));
            this.vm.Status = "Reading...";
            t.Start(s);
        }


        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //(sender as DataGrid).ScrollIntoView(e.Row.Item);
        }

        public string CreateContact(Contact contact)
        {
            String vcfName = contact.ContactName;
            if (contact.ContactName.Contains(" "))
            {
                int ndx = contact.ContactName.LastIndexOf(" ");
                vcfName = contact.ContactName.Substring(ndx + 1, contact.ContactName.Length - 1-ndx) + ";" + contact.ContactName.Substring(0, ndx);
            }

            var phoneLines=contact.Phone.Split(new string[1]{ Environment.NewLine},StringSplitOptions.RemoveEmptyEntries);
            string phones = "";
            foreach (var phone in phoneLines)
            {
                phones += "TEL; CELL:" + phone + Environment.NewLine;
            }            
            vcfName = vcfName.Replace(' ',';');
            string vcfContact = String.Format(Environment.NewLine+@"BEGIN:VCARD
VERSION:3.0
N;CHARSET=utf-8:{0};;{1}
{2}END:VCARD",vcfName, vcfName.Count(c=> { return c == ';'; })>1?"":";",phones);
            return vcfContact;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "*.*|*.vcf";
            sd.FileName =Environment.CurrentDirectory+ "\\VCF_CardManager_contacts.vcf";           
            sd.ShowDialog();
            if(String.IsNullOrEmpty(sd.FileName))
                return;
            vm.FileName=sd.FileName;
            
            StringBuilder sb = new StringBuilder();
            foreach (Contact contact in vm.Contacts.Where(w=>w.IsRequired))
            {
                String sContact=CreateContact(contact);
                sb.Append(sContact);                                
            }
            File.WriteAllBytes(sd.FileName, UTF8Encoding.UTF8.GetBytes(sb.ToString().Substring(sb.ToString().IndexOf("BEGIN:VCARD"))));

        }

    }

    public class ContactVM : INotifyPropertyChanged
    {
        public ObservableCollection<Contact> Contacts { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private String _fileName;
        public String FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                NotifyPropertyChanged("FileName");
            }
        }
        private String _status;
        public String Status
        {
            get { return _status; }
            set
            {
                _status = value;
                NotifyPropertyChanged("Status");
            }
        }
        public ContactVM()
        {
            this.Status = ".";
            this.Contacts = new ObservableCollection<Contact>();
            //this.Contacts.Add(new Contact()
            //{
            //    ContactName = "Dummy",
            //    Phone = "123456789"
            //});
            this.Contacts.CollectionChanged += (obj, prms) =>
            {
                System.Diagnostics.Debug.Write(prms);
            };
        }

        private void NotifyPropertyChanged(String property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }



    public class Contact : INotifyPropertyChanged
    {
        private string _contactName;
        private string _phone;
        private bool _isRequired;
        private int _itemNo;

        public event PropertyChangedEventHandler PropertyChanged;

        public String ContactName { get { return _contactName; } set { _contactName = value; NotifyPropertyChanged("ContactName"); } }
        public String Phone { get { return _phone; } set { _phone = value; NotifyPropertyChanged("Phone"); } }
        public bool IsRequired { get { return _isRequired; } set { _isRequired = value; NotifyPropertyChanged("IsRequired"); } }
        public int ItemNo { get { return _itemNo; } set { _itemNo = value; NotifyPropertyChanged("ItemNo"); } }

        private void NotifyPropertyChanged(String property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
