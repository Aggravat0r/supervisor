using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;

namespace OC
{
    public class Core
    {
        public static readonly int PSTATUS_WAIT = 0;
        public static readonly int PSTATUS_RUN = 1;
        public static readonly int PSTATUS_SLEEP = 2;
        public static readonly int PSTATUS_ZOMBIE = 3;
        public static readonly int PCOUNT_MAX = 100;

        public static readonly int MEMORY_COUNT = 32;

        public static List<Semaphore> sem; // список семафоров в системе
        public static List<Process> proc; //список процессов в системе
        public static List<page> mem; // список страниц в памяти

        public static int csem = 0; // колво семафоров в системе


        public Core(int procC)
        {
            mem = new List<page>(MEMORY_COUNT);
            for (int i = 0; i < MEMORY_COUNT; i++)
            {
                page pr;
                mem.Add(pr = null);
            }
            proc = new List<Process>(procC);
            for (int i = 0; i < procC; i++)
            {
                Process pr;
                proc.Add(pr=null);
            }
            sem = new List<Semaphore>(100);
            for (int i = 0; i < 100; i++)
            {
                Semaphore pr;
                sem.Add(pr = null);
            }
        }       

        public void Planning()
        {
            while(proc.Count>0)
            {
                for (int i = 0; i < proc.Count; i++)
                {
                    if (proc[i].TimeWait>5)
                    {
                        proc[i].Priory += 5;
                    }
                }
               /*
                * отсортировать по полю Priory
                * поискать в интернете использование ICompare (IComparer)
                * */
               // IComparer myComparer = new Process.myReverserClass();

                proc.Sort(delegate(Process a, Process b)
                {
                    return b.Priory.CompareTo(a.Priory);
                });

               // proc.Sort(Process.ComparePriory);
                Process CurrProc = proc.First(); // взять первый процесс
                Print(); // распечатать таблицу
                int result=CurrProc.Run(); //выполнить процесс
                if (result > 0) // процесс не закончил работать
                {
                    proc[0].Priory -= 20; // уменьшить приоритет
                    proc[0].Time -= result; // уменьшить время выполнения
                }
                else
                {
                    function_process.DeleteProcess(proc[0].PID); // удалить процесс
                }
                foreach (Process t in proc)
                {
                    t.TimeWait += 1;
                }

            }
            
        }
        public static void Print()
        {
            /*proc.Sort(delegate(Process a, Process b)
            {
                return b.Priory.CompareTo(a.Priory);
            });*/
           // Form1.Invoke(delegate Print());
            Program.myForm.Invoke((Action)delegate() 
                {          
                    Program.myForm.Log.Text += "\n" +"PID" + "  " + "PPID" + "  " + "Priory" + "  " + "name" + "  " + "Status" +"\n";
                    Program.myForm.Log.Text += "----------------------------------------------------------------------------" + "\n";
                    for (int i = 0; i < proc.Count; i++)
                    {
                        Program.myForm.Log.Text += proc[i].PID + "  " + proc[i].PPID + "  " + proc[i].Priory + "  " + proc[i].name + "  " + proc[i].Status + "\n";
                    }
            
               });
        }

        public void init()
        {
            function_process.CreateProcess(Main.Sess.user_id,50,"slash",5);
            function_process.CreateProcess(Main.Sess.user_id, 60, "clears", 10);
            function_process.CreateProcess(Main.Sess.user_id, 60, "chuma", 13);
            function_process.CreateProcess(Main.Sess.user_id, 90, "Apocalyptik", 9);
        }


    }    

    public class Process //: IComparable
    {     
        public int PID; // ID процесса
        public int PPID; // ID пользователя, который его возвал
        public int Priory; // приоритет
        public string name; // имя поцесса
        public int Time; // время выполнения
        public int Status; // статус процесса
        public List<int> Memory=new List<int>(); // куда обращается процесс
        public int TimeWait; // время простоя

        public Process(int ID, int userID, int pPriory, int ptime, string pname, int sect_memory)
        {
            PID = ID;
            PPID = userID;
            Priory = pPriory;
            Time = ptime;
            name = pname;
            Status = 0;
            Memory.Add(sect_memory);
            TimeWait = 0;
        }
        public int Run()
        {
            int quant = 10;
            this.TimeWait = 0; // сбрасываем время простоя
            /*
             * проверкаа семафора на занятость. если семафор занят - сразу на выход
             * если семафор не занят, то начать выполнение, если есть кванты
             * */
            for (int i = 0; i < this.Memory.Count; i++)
            {
                if (/*(Core.mem[this.Memory[i]].locking.free == false) && */(Core.mem[this.Memory[i]].process[0] == this.PID))
                {
                    while (quant != 0)
                    {
                        Program.myForm.Log.Text += "\n" + this.name+": Процесс выполняется..." + "\n";
                        quant--;
                        this.Time--;
                        // Thread.Sleep(1000);
                    }
                    if (this.Time > quant)
                    {
                        Program.myForm.Log.Text += "\n" + this.name + ": Процесс до конца не выполнился и вернется в список процессов." + "\n";
                        return this.Time - quant;
                    }
                    else
                    {
                        Program.myForm.Log.Text += "\n" + this.name + ": Процесс до конца выполнился и будет удален." + "\n";
                        return 0;
                    }
                }
                else 
                {
                    Program.myForm.Log.Text += "\n" + this.name + ": Область памяти занята." + "\n";
                    return this.Time;
                }
            }            
            return 0;
        }
    }

    public class Semaphore
    {
        public int ID;
        public Boolean free; // индикатор занятости 

        public Semaphore(int i)
        {
            ID = i;
            free = true;
        }

    }

    public class funtion_semaphore
    {
        public static void CreateSemaphore(int curPID)
        {
            Core.csem++;
            Semaphore t = new Semaphore(curPID); // создаем семафор с id 
            Core.sem.Add(t); // добавляем семафор в список
            Core.mem[curPID] = new page();
            Core.mem[curPID].id = curPID; // устанавливаем области памяти id
            Core.mem[curPID].locking = t; // связываем с определенным семафором
            Core.mem[curPID].process.Add(curPID); //процесс, который использует память
            //Core.mem[i].pid = curPID; // связываем с определенным процессом
            //return i;
        }
               
    }
    public class function_process
    {
        static int SearchFreePID()
        {
            int res = -1;
            for (int i = 0; i < Core.proc.Count; i++)
            {
                if (Core.proc[i] == null)
                {
                    res = i;
                    break;
                }
            }
            return res;
        }

        public static int CreateProcess(int ppid, int prior, string name, int ms)
        {
            int sect_memory=-1;
            int curPID = SearchFreePID();
            if (curPID < 0)
                return -1;
            for (int i = 0; i < Core.mem.Count(); i++)
            {
                if (Core.mem[i] == null)
                {
                    sect_memory = i;
                    break;
                }
            }
            if (sect_memory < 0)
                return -1;
            Core.proc[curPID] = new Process(curPID, ppid, prior, ms, name,sect_memory);
            if (ms >= 0)
            {
                funtion_semaphore.CreateSemaphore(sect_memory); // добавляет семафор к участку памяти 
                Core.mem[sect_memory].locking.free = false;
            }
           /* if (ms < 5)
                Core.proc[curPID].Priory = 90;
            else
                if (ms >= 5)
                    Core.proc[curPID].Priory = 70;*/
            
            return curPID;
        }

        public static void DeleteProcess(int pid)
        {
            for (int j = 0; j < Core.proc[0].Memory.Count; j++) // проверяем список областей памяти
            {            
                if (Core.mem[(Core.proc[0].Memory[j])].locking.free==false) // если семафор занят
                {
                    Core.mem[(Core.proc[0].Memory[j])].locking.free = true; // освободить его
                }
                for (int i=0; i<Core.mem[(Core.proc[0].Memory[j])].process.Count(); i++) // убираем из памяти - списка процессов, использующих память, удаляемый процесс
                {
                    if (Core.mem[(Core.proc[0].Memory[j])].process[i]==pid) // если такой процесс в списке есть 
                        Core.mem[(Core.proc[0].Memory[j])].process.Remove(pid); // удалить его 
                }

            }
            if (Core.proc[0] != null)
            {
                Core.proc[0].Status = Core.PSTATUS_ZOMBIE;
                Core.proc.Remove(Core.proc[0]);
            }
        }

        public static int Select()
        {
            int maxp = Core.proc[0].Priory;
            int maxi = 0;

            for (int i = 1; i < Core.PCOUNT_MAX; i++)
            {
                if ((Core.proc[i] != null) && (maxp < Core.proc[i].Priory) && (Core.proc[i].Status == 0))
                {
                    maxp = Core.proc[i].Priory;
                    maxi = i;
                }
            }

            return maxi;
        }
        
    }

    public class page
    {
        public int id; // идентификатор страницы
        public Semaphore locking; // признак занятости страницы
        public List<int> process=new List<int>(); // какие процессы работают с памятью
        public string data; // что-то ж должно бытьь записано...
    }

}

/*


/*vipolnit operaciu s semaforom
int semop(int desc, int op)
{if (desc<0 || desc>=SEM_NUM)
  {puts("Incorrect desk in semop."); return -1;}
 sem[desc].val += op;
 if (sem[desc].val>0) return 0; /*yspex
 else return -1;
}

/*ypravlenie semaforom
int semctl(int desc, int cmd, str_sem arg)
{int correct_arg(str_sem);
 if ((cmd!=IPC_RMD) && (cmd!=IPC_SET))
  {puts("Incorrect cmd in semctl."); return -1;}
 if (desc<0 || desc>=SEM_NUM)
  {puts("Incorrect desk in semctl."); return -1;}
 if (cmd==IPC_RMD)
  {sem[desc].sid=0;
   sem[desc].val=0;
   return 0; /*yspex
  }
 else
  if (correct_arg(arg))
   {sem[desc]=arg;
    return 0; /*yspex
   }
  else
   {puts("Incorrect arg in semctl."); return -1;}
}

/*proverka korrektnosti
int correct_arg(str_sem arg)
{ if ((arg.sid==SEM_RD) || (arg.sid==SEM_WR)
       || (arg.sid==SEM_C))
  return 1;     /*korrektno
  else return 0;
}

//-----------razdeljaemaja pamjat'------------------------
/*sozdat ili otkrit
int shmget(int pid/*, int size*//*, int flag)
{int i;
 if ((flag!=IPC_CREATE) && (flag!=IPC_OPEN))
  {puts("Incorrect flag in shmget"); return -1;}
 if (flag==IPC_CREATE)
   {for(i=0; i<PAGE_NUM; i++)
     if(!shm[i].lock) break; // s i-togo bloka stranici svobodni
    if(i==PAGE_NUM)
     {puts("Incorrect pid in shmget"); return -1;}
    return i;
    }
 else //IPC_OPEN
  {for(i=0; i<PAGE_NUM; i++)
    if (shm[i].pid==pid) break;
   if(i==PAGE_NUM)
    {puts("Incorrect pid in shmget"); return -1;}
   return i; /*yspex
  }
}

/*prisoedeit oblast' pamjati
void shmat(int desc, int pid)
{if (desc<0 || desc>=PAGE_NUM)
  {puts("Incorrect desk in shmat.");
   return;
   }
 shm[desc].pid=pid;
 shm[desc].lock=1;
}

*/