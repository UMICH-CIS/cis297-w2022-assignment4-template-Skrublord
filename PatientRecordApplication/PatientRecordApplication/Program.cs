using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;

class Patient
{
   public int PatientID { get; set; }
   public string PatientName { get; set; }
   public double OutstandingBalance { get; set; }
}

class Program
{
    //runs the main menu until they enter 5 on the main menu to exit
    static void Main(string[] args)
    {
        bool showMenu = true;
        while (showMenu)
        {
            showMenu = MainMenu();
        }
    }

    //Provides user with a main menu to select the function to be used
    //user will be sent back here until application is quit out of
    private static bool MainMenu()
    {
        Console.Clear();
        Console.WriteLine("Choose an option:");
        Console.WriteLine("1) Write Data");
        Console.WriteLine("2) Read All Data");
        Console.WriteLine("3) Read Specific Data");
        Console.WriteLine("4) Check all Min balance due");
        Console.WriteLine("5) Exit");
        Console.Write("\r\nSelect an option: ");

        switch (Console.ReadLine())
        {
            case "1":
                WritePatientRecords.WriteRecords();
                return true;
            case "2":
                ReadPatientRecords.ReadRecords();
                return true;
            case "3":                
                try {
                    ReadSpecificPatientRecords.FindRecords();
                } 
                catch(PatientSearchFailedException e) {
                    Console.WriteLine("User defined exception: {0}", e.Message);
                }
                Console.ReadKey();
                return true;
            case "4":
                MinBalance.FindBalance();
                return true;
            case "5":
                return false;

            default:
                return true;
        }
    }
}

class WritePatientRecords
{
    //Writes data sequentially to Patient.txt file in project folder
    public static void WriteRecords()
    {
        const int END = 999;
        const string DELIM = ",";
        const string FILENAME = "Patient.txt";
        Patient patient = new Patient();
        FileStream outFile = new FileStream(FILENAME, FileMode.Create, FileAccess.Write);
        StreamWriter writer = new StreamWriter(outFile);
        Write("Enter patient ID " + END + " to quit >> ");        
        try
        {
            patient.PatientID = Convert.ToInt32(ReadLine());
        }
        catch (FormatException)
        {
            Console.WriteLine("ID provided was not an integer, PatientID must be an integer");
        }
        finally
        {
            Write("Enter patient ID " + END + " to quit >> ");
            patient.PatientID = Convert.ToInt32(ReadLine());
        }         
        
        while (patient.PatientID != END)
        {
            Write("Enter patient name >> ");
            patient.PatientName = ReadLine();
            Write("Enter patient balance >> ");
            try
            {
                patient.OutstandingBalance = Convert.ToDouble(ReadLine());
            }
            catch (FormatException)
            {
                Console.WriteLine("Balance provided was not an integer/double, OutstandingBalance must be an integer or double");
            }
            finally
            {
                Write("Enter patient balance >> ");
                patient.OutstandingBalance = Convert.ToDouble(ReadLine());
            }
            writer.WriteLine(patient.PatientID + DELIM + patient.PatientName + DELIM + patient.OutstandingBalance);
            Write("Enter next patient ID or " + END + " to quit >> ");
            patient.PatientID = Convert.ToInt32(ReadLine());
        }
        writer.Close();
        outFile.Close();
    }
}

class ReadPatientRecords
{
    //Read data from a Sequential Access File
    public static void ReadRecords()
    {
        const char DELIM = ',';
        const string FILENAME = "Patient.txt";
        Patient patient = new Patient();
        FileStream inFile = new FileStream(FILENAME, FileMode.Open, FileAccess.Read);
        StreamReader reader = new StreamReader(inFile);
        string recordIn;
        string[] fields;
        WriteLine("\n{0,-5}{1,-12}{2,8}\n", "Num", "Name", "Balance");
        recordIn = reader.ReadLine();
        while (recordIn != null)
        {
            fields = recordIn.Split(DELIM);
            patient.PatientID = Convert.ToInt32(fields[0]);
            patient.PatientName = fields[1];
            patient.OutstandingBalance = Convert.ToDouble(fields[2]);
            WriteLine("{0,-5}{1,-12}{2,10}", patient.PatientID, patient.PatientName, patient.OutstandingBalance.ToString("C"));
            recordIn = reader.ReadLine();
        }
        Console.Write("\r\nPress Enter to return to Main Menu");
        Console.ReadLine();
        reader.Close();
        inFile.Close();
    }
}

class ReadSpecificPatientRecords
{
    //Reads through patients file to find a specific patient ID,
    //if not found, throw custom exception
    public static void FindRecords()
    {
        const char DELIM = ',';
        const string FILENAME = "Patient.txt";
        Patient patient = new Patient();
        FileStream inFile = new FileStream(FILENAME, FileMode.Open, FileAccess.Read);
        StreamReader reader = new StreamReader(inFile);
        string recordIn;
        string[] fields;
        string request;
        bool found = false;
        Write("Enter patient ID number to find >> ");
        request = ReadLine();
        WriteLine("\n{0,-5}{1,-12}{2,10}\n", "Num", "Name", "Balance");
        recordIn = reader.ReadLine();
        while (recordIn != null)
        {
            fields = recordIn.Split(DELIM);
            patient.PatientID = Convert.ToInt32(fields[0]);
            patient.PatientName = fields[1];
            patient.OutstandingBalance = Convert.ToDouble(fields[2]);
            if(fields[0].Equals(request))
            {
                WriteLine("{0,-5}{1,-12}{2,10}", patient.PatientID, patient.PatientName,patient.OutstandingBalance.ToString("C"));
                found = true;
            }
            recordIn = reader.ReadLine();
        }
        if(!found)
            throw(new PatientSearchFailedException("Patient ID not found"));
        Console.Write("\r\nPress Enter to return to Main Menu");
        Console.ReadLine();
        reader.Close();
        inFile.Close();
    }
}

class MinBalance
{
    //repeatedly searches a file to produce 
    //lists of patients who meet a minimum balance requirement specified by user
    public static void FindBalance()
    {
        const char DELIM = ',';
        const int END = 999;
        const string FILENAME = "Patient.txt";
        Patient patient = new Patient();
        FileStream inFile = new FileStream(FILENAME, FileMode.Open, FileAccess.Read);
        StreamReader reader = new StreamReader(inFile);
        string recordIn;
        string[] fields;
        double minBalance;
        Write("Enter minimum balance to find or " + END + " to quit >> ");
        minBalance = Convert.ToDouble(Console.ReadLine());
        while (minBalance != END)
        {
            WriteLine("\n{0,-5}{1,-12}{2,10}\n", "Num", "Name", "Balance");
            inFile.Seek(0, SeekOrigin.Begin);
            recordIn = reader.ReadLine();
            while (recordIn != null)
            {
                fields = recordIn.Split(DELIM);
                patient.PatientID = Convert.ToInt32(fields[0]);
                patient.PatientName = fields[1];
                patient.OutstandingBalance = Convert.ToDouble(fields[2]);
                if (patient.OutstandingBalance >= minBalance)
                    WriteLine("{0,-5}{1,-12}{2,8}", patient.PatientID, patient.PatientName, patient.OutstandingBalance.ToString("C"));
                recordIn = reader.ReadLine();
            }
            Write("\nEnter minimum balance to find or " + END + " to quit >> ");
            minBalance = Convert.ToDouble(Console.ReadLine());
        }
        reader.Close();
        inFile.Close(); 
    }
}

public class PatientSearchFailedException: Exception {
   public PatientSearchFailedException(string message): base(message) {
   }
}
