﻿using FluentExpressionSQL;
using FluentExpressionSQL.Mapper;
using Expression2SqlTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using Pure.Data.Pooling;
//using Pure.Data.Pooling.Impl;

namespace Pure.Data.Test
{
    public class PoolingTest:IDisposable
    {

        public static void Test()
        {


            string title = "PoolingTest";
            Console.Title = title;

            CodeTimer.Time(title, 2, () =>
            {
                //TestGetAndReturn();
                //TestRun();
                //TestDbContext();
                TestGetAndReturnPoolDatabase();
                //TestDbContext();
            });


            new Thread(() =>
            {

                Thread.Sleep((10000));
                string info = "";
                //string info = "--------------- databaseWrapperPool --------------- \r\n" + databaseWrapperPool.ShowStatisticsInfo();
                //info += "\r\n";
                //info += "\r\n";
                //Log(info, null, MessageType.Debug);

                info = "--------------- databasePool --------------- \r\n" + databasePool.ShowStatisticsInfo();
                info += "\r\n";
                info += "\r\n";

                Log(info, null, MessageType.Debug);

            }).Start();


          
            Console.Read();


        }
        //public static void TestRun()
        //{

        //    var s = new GenericPoolSample();
        //    //s.Test();
        //    //Console.WriteLine("test is finish");
        //    //Console.ReadKey();
        //    s.TestParalle();
        //    Console.WriteLine("Finish");
        //    Console.ReadLine();

        //}
        //public static void TestPoolWithKey()
        //{
        //    var hashSet = new HashSet<int>();
        //    var pool = new GenericKeyedObjectPool<string, VerifyStruct>(new VerifyStructFactory(), new GenericKeyedObjectPoolConfig() { TestOnBorrow = true, TestOnReturn = true });
        //    for (int i = 0; i < 100; i++)
        //    {
        //        var obj = pool.BorrowObject("test");

        //        Console.WriteLine("test is BorrowObject-" + obj.CreateTime);
        //        pool.ReturnObject("test", obj);
        //        hashSet.Add(obj.GetHashCode());
        //        Console.WriteLine("Finish");
        //        Thread.Sleep(1000);
        //    }

        //}
        public static void Log(string msg, Exception ex, MessageType type = MessageType.Debug)
        {
            ConsoleHelper.Instance.OutputMessage(msg, ex, type);
            //FastLogger.WriteLog(msg);
        }

        public static DatabasePool databasePool = new DatabasePool(
            new DatabasePoolPolicy() {
                CreateFactory = ()=> new PooledDatabase("PureDataConfiguration.xml", Log,
                                 config => {
                                 }),
                AsyncCreateFactory = null,
                LogAction = Log,
                MaxPoolSize = 16,
                EnableDiagnostics = true,
                EvictionSettings = EvictionSettings.Default,
                EvictionTimer = null,
                
                OnValidateObject = (ctx) =>
                {
                    if (ctx.Direction == Pooling.PooledObjectDirection.Inbound)
                    {
                        return true;
                    }
                    else
                    {
                        return true;
                    }
                },
                OnReleaseResource = (o) =>
                {
                    var resource = o as PooledDatabase;
                    Log("Release -> " + resource.GetHashCode() + " , connection: " + resource.Connection.GetHashCode() + ", status : " + resource.Connection.State, null, MessageType.Error);
                    //resource?.Close();
                },
                OnResetState = (o) =>
                {
                    var resource = o as PooledDatabase;

                    Log("Reset -> " + resource.GetHashCode() + " , connection: " + resource.Connection.GetHashCode() + ", status : " + resource.Connection.State, null, MessageType.Info);
                    //resource?.Close();
                },
                OnCreateResource = (o) =>
                {
                    var resource = o as PooledDatabase;
                    Log("Create -> " + resource.GetHashCode() + " , connection: " + resource.Connection.GetHashCode() + ", status : " + resource.Connection.State, null, MessageType.Warning);
                },
                OnEvictResource = (o) =>
                {
                    var resource = o as PooledDatabase;

                    Log("Evict -> " + resource.GetHashCode() + " , connection: " + resource.Connection.GetHashCode() + ", status : " + resource.Connection.State, null, MessageType.Error);
                    //resource?.Close();
                },
                OnGetResource = (o) =>
                {
                    var resource = o as PooledDatabase;
                    Log("Get -> " + resource.GetHashCode() + " , connection: " + resource.Connection.GetHashCode() + ", status : " + resource.Connection.State, null, MessageType.Info);
                },
                OnReturnResource = (o) =>
                {
                    var resource = o as PooledDatabase;
                    Log("Return -> " + resource.GetHashCode() + " , connection: " + resource.Connection.GetHashCode() + ", status : " + resource.Connection.State, null, MessageType.Warning);
                }
            }
           );


        //public static DatabasePool databasePool = new DatabasePool(() => new PooledDatabase("PureDataConfiguration.xml", Log,
        //config => {
        //})
        //{
            
        //    LogAction = Log,
        //    OnValidateObject = (ctx) =>
        //    {
        //        if (ctx.Direction == Pooling.PooledObjectDirection.Inbound)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return true;
        //        }
        //    },
        //    OnReleaseResources = (o) =>
        //    {
        //        var resource = o as PooledDatabase;
        //        Log("Release -> " + resource.GetHashCode() + " , connection: " + resource.Connection.GetHashCode() + ", status : " + resource.Connection.State, null, MessageType.Error);
        //        //resource?.Close();
        //    },
        //    OnResetState = (o) =>
        //    {
        //        var resource = o as PooledDatabase;

        //        Log("Reset -> " + resource.GetHashCode() + " , connection: " + resource.Connection.GetHashCode() + ", status : " + resource.Connection.State, null, MessageType.Info);
        //        //resource?.Close();
        //    },
        //    OnCreateResource = (o) =>
        //    {
        //        var resource = o as PooledDatabase;

        //        Log("Create -> " + resource.GetHashCode() + " , connection: " + resource.Connection.GetHashCode() + ", status : " + resource.Connection.State, null, MessageType.Warning);
        //    },
        //    OnEvictResource = (o) =>
        //    {
        //        var resource = o as PooledDatabase;

        //        Log("Evict -> " + resource.GetHashCode() + " , connection: " + resource.Connection.GetHashCode() + ", status : " + resource.Connection.State, null, MessageType.Error);
        //        //resource?.Close();
        //    },
        //    OnGetResource = (o) =>
        //    {
        //        var resource = o as PooledDatabase;
        //        Log("Get -> " + resource.GetHashCode() + " , connection: " + resource.Connection.GetHashCode() + ", status : " + resource.Connection.State, null, MessageType.Info);
        //    },
        //    OnReturnResource = (o) =>
        //    {
        //        var resource = o as PooledDatabase;
        //        Log("Return -> " + resource.GetHashCode() + " , connection: " + resource.Connection.GetHashCode() + ", status : " + resource.Connection.State, null, MessageType.Warning);
        //    },


        //}, Environment.ProcessorCount * 2
        //   //, new Pooling.EvictionSettings() { Enabled = true, Period = TimeSpan.FromMilliseconds(1000) }
        //   );








        //public static DatabaseWrapperPool databaseWrapperPool = new DatabaseWrapperPool(() => new Pooling.PooledObjectWrapper<IDatabase>(new Database("PureDataConfiguration.xml", Log,
        // config => {

        // })) {
        //    LogAction = Log,
        //    OnValidateObject = (ctx) =>
        //    {
        //        if (ctx.Direction == Pooling.PooledObjectDirection.Inbound)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return true;
        //        }
        //    },
        //    OnReleaseResources = (resource) => { Log("Release -> " + resource.GetHashCode() +" , connection: "+ resource.Connection.GetHashCode() +", status : "+resource.Connection.State, null, MessageType.Error);
             
        //    },
        //    OnResetState = (resource) => { Log("Reset -> " + resource.GetHashCode() + " , connection: " + resource.Connection.GetHashCode() + ", status : " + resource.Connection.State, null, MessageType.Info);
                
        //    },
        //    OnCreateResource = (resource) => { Log("Create -> "+ resource.GetHashCode() + " , connection: " + resource.Connection.GetHashCode() + ", status : " + resource.Connection.State, null, MessageType.Warning); },
        //    OnEvictResource = (resource) => { Log("Evict -> " + resource.GetHashCode() + " , connection: " + resource.Connection.GetHashCode() + ", status : " + resource.Connection.State, null, MessageType.Error);
                 
        //    },
        //    OnGetResource = (resource) => { Log("Get -> " + resource.GetHashCode() + " , connection: " + resource.Connection.GetHashCode() + ", status : " + resource.Connection.State, null, MessageType.Info); },
        //    OnReturnResource = (resource) => { Log("Return -> " + resource.GetHashCode() + " , connection: " + resource.Connection.GetHashCode() + ", status : " + resource.Connection.State, null, MessageType.Warning); },


        //}, Environment.ProcessorCount * 2
        //    , new Pooling.EvictionSettings() { Enabled = true, Period =TimeSpan.FromMilliseconds(1000)}
        //    );


        public static void TestGetAndReturnPoolDatabase()
        {
            //var db11 = DbMocker.NewDataBase();

            //var userinfo = db11.Get<UserInfo>(9);
            //userinfo.Role = RoleType.管理员;
            //userinfo.DTCreate = DateTime.Now;

            //db11.Update(userinfo);
            //var olist = db11.GetAll<UserInfo>();




            var random = new Random();
            //var pdb = databasePool.GetPooledDatabase();
        
            for (var a = 0; a < 10; a++)
            {
                new Thread(() =>
                {

                    Thread.Sleep(random.Next(100));

                    for (var b = 0; b < 1; b++)
                    {
                        var db = databasePool.GetPooledDatabase();
                        //using (var db = databasePool.GetPooledDatabase())
                        //{
                       
                            Log("GetCurrentDatabase:" + db.GetHashCode().ToString(), null);
                            db.Get<UserInfo>(1);
                            var id =   db.ExecuteScalar<int>("select 1 from TB_USER");
                            db.UpdateBySQL("update TB_USER set Name ='2333'  where id=" + id, null);

                            db.FirstOrDefault<UserInfo>(p=>p.Id == 123);

                            db.ExecuteList<UserInfo>("select * from TB_USER");
                            Thread.Sleep(random.Next(50));

                            db.ExecuteExpandoObjects("select * from TB_USER");

                            Log("ReturnDatabase:" + db.GetHashCode().ToString(), null);
                        db.ReturnPooledDatabase(); 
                        //}

                    }

                    for (var b = 0; b < 5; b++)
                    {

                        Task task = new Task(async ()=> {
                            var db = databasePool.GetPooledDatabase();
                            //using (var db = databasePool.GetPooledDatabase())
                            //{
                                Log("GetCurrentDatabase:" + db.GetHashCode().ToString(), null);
                                await db.GetAsync<UserInfo>(1);
                                var id = db.ExecuteScalar<int>("select 1 from TB_USER");
                                db.UpdateBySQL("update TB_USER set Name ='2333'  where id=" + id, null);

                                db.FirstOrDefault<UserInfo>(p => p.Id == 123);

                                await db.ExecuteListAsync<UserInfo>("select * from TB_USER");
                                Thread.Sleep(random.Next(50));

                                await db.ExecuteExpandoObjectsAsync("select * from TB_USER");

                                Log("ReturnDatabase:" + db.GetHashCode().ToString(), null);
                        db.ReturnPooledDatabase();
                            //}
                        });
                        task.Start();



                    }

                    Log("filish thread:" + a.ToString(), null);


                }).Start();
            }

            //pdb.RunInAction(() =>
            //{
            //    pdb.SetConnectionAlive(true);
            //    var userinfo9 = pdb.Get<UserInfo>(9);
            //    var userinfo = new UserInfo() { Id = 9, Name = "zss", Age = 20, HasDelete = false, StatusCode = 1 };
            //    pdb.Insert<UserInfo>(userinfo);
            //    userinfo.Role = RoleType.管理员;
            //    userinfo.DTCreate = DateTime.Now;

            //    //pdb.Update(userinfo);

            //    var oo2 = pdb.FirstOrDefault<UserInfo>(p => p.Name == "zss");
            //    //var oo4 = pdb.Query<UserInfo>(p => p.Name == "zss");

            //    //pdb.Delete<UserInfo>(p => p.Name == "zss");

            //    //oo2 = pdb.FirstOrDefault<UserInfo>(p => p.Name == "zss");

            //    //pdb.CommitTransaction();
            //    pdb.SetConnectionAlive(true);

            //}); 

            //using (var pdb = databasePool.GetPooledDatabase())
            //{
            //    //pdb.BeginTransaction();
            //    //var oo = pdb.FirstOrDefault<UserInfo>(p => p.Id == 9);
            //    var userinfo9 = pdb.Get<UserInfo>(9);
            //    var userinfo = new UserInfo() { Id = 9, Name = "zss", Age = 20, HasDelete = false, StatusCode = 1 };
            //    pdb.Insert<UserInfo>(userinfo);
            //    userinfo.Role = RoleType.管理员;
            //    userinfo.DTCreate = DateTime.Now;

            //    //pdb.Update(userinfo);

            //    var oo2 = pdb.FirstOrDefault<UserInfo>(p => p.Name == "zss");
            //    //var oo4 = pdb.Query<UserInfo>(p => p.Name == "zss");

            //    //pdb.Delete<UserInfo>(p => p.Name == "zss");

            //    //oo2 = pdb.FirstOrDefault<UserInfo>(p => p.Name == "zss");

            //    //pdb.CommitTransaction();

            //    pdb.ReturnPooledDatabase();
            //    //pdb.Close();
            //}


            //using (var pdb = databasePool.GetPooledDatabase())
            //{
            //    //var oo = pdb.FirstOrDefault<UserInfo>(p => p.Id == 9);
            //    var userinfo9 = pdb.Get<UserInfo>(9);
            //    var userinfo = new UserInfo() { Id = 9, Name = "zss", Age = 20, HasDelete = false, StatusCode = 1 };
            //    pdb.Insert<UserInfo>(userinfo);
            //    userinfo.Role = RoleType.管理员;
            //    userinfo.DTCreate = DateTime.Now;

            //    pdb.Update(userinfo);

            //    var oo2 = pdb.FirstOrDefault<UserInfo>(p => p.Name == "zss");
            //    var oo4 = pdb.Query<UserInfo>(p => p.Name == "zss");

            //    pdb.Delete<UserInfo>(p => p.Name == "zss");

            //    oo2 = pdb.FirstOrDefault<UserInfo>(p => p.Name == "zss");

            //    //pdb.ReturnPooledDatabase();

            //    //pdb.Close();
            //}

        }


        //public static void TestGetAndReturn() {

        //    var db = databaseWrapperPool.GetPooledDatabaseWrapper().InternalResource;
        //    Log("GetCurrentDatabase:" + db.GetHashCode().ToString(), null);
        //    databaseWrapperPool.ReturnPooledDatabase(db);
        //    Log("ReturnDatabase:" + db.GetHashCode().ToString(), null);

        //    db = databaseWrapperPool.GetPooledDatabaseWrapper().InternalResource;
        //    Log("GetCurrentDatabase:" + db.GetHashCode().ToString(), null);
        //    databaseWrapperPool.ReturnPooledDatabase(db);
        //    Log("ReturnDatabase:" + db.GetHashCode().ToString(), null);
 


        //}

        private static void ExeTExt()
        {

            //o.DTCreate = DateTime.Now;
            //db.Update<UserInfo>(o);

            //using (var db = pooldb.GetCurrentDatabase())
            //{
            //    //Thread.Sleep(100);

            //}




            //using (var pdb = databaseWrapperPool.GetPooledDatabaseWrapper())
            //{
            //    //var oo = pdb.InternalResource.FirstOrDefault<UserInfo>(p => p.Id == 9);
            //    var oo = pdb.InternalResource.Get<UserInfo>(9);

            //    pdb.InternalResource.Close();
            //}

            using (var pdb = databasePool.GetPooledDatabase())
            { 
                //var oo = pdb.FirstOrDefault<UserInfo>(p => p.Id == 9);
                var userinfo9 = pdb.Get<UserInfo>(9);
                //var userinfo = new UserInfo() { Id = 9, Name = "zss", Age = 20, HasDelete = false, StatusCode = 1 };
                //pdb.Insert<UserInfo>(userinfo);
                //userinfo.Role = RoleType.管理员;
                //userinfo.DTCreate = DateTime.Now;

                //pdb.Update(userinfo);

                var oo2 = pdb.FirstOrDefault<UserInfo>(p => p.Name == "zss");
                var oo4 = pdb.Query<UserInfo>(p => p.Name == "zss");

                //pdb.Delete<UserInfo>(p => p.Name == "zss");

                //oo2 = pdb.FirstOrDefault<UserInfo>(p => p.Name == "zss");

                //pdb.ReturnPooledDatabase();

            }
           



                //pdb.Close();
                //var db = pooldb.GetPooledDatabase().InternalResource;
                //Log("GetCurrentDatabase:" + db.GetHashCode().ToString(), null);
                ////Thread.Sleep(100);
                //pooldb.ReturnPooledDatabase(db);
                ////pooldb.Close();
                //Log("ReturnDatabase:" + db.GetHashCode().ToString(), null);


        }
        public static void TestDbContext()
        {
            // ExeTExt();
            //Thread.Sleep(1000);
            Parallel.Invoke(new ParallelOptions() { MaxDegreeOfParallelism = 5 },
                  () =>
                  {
                      ExeTExt();

                  }
                  , () =>
                  {
                      ExeTExt();
                  }
                  , () =>
                  {
                      ExeTExt();

                  }
                  , () =>
                  {
                      ExeTExt();

                  }
                  , () =>
                  {
                      ExeTExt();

                  }, () =>
                  {
                      ExeTExt();

                  }, () =>
                  {
                      ExeTExt();

                  }, () =>
                  {
                      ExeTExt();

                  }, () =>
                  {
                      ExeTExt();

                  }, () =>
                  {
                      ExeTExt();

                  }



              /////////////////////////////////////
              //() =>
              //{
              //    ExeTExt();

              //}, () =>
              //{
              //    ExeTExt();

              //}, () =>
              //{
              //    ExeTExt();

              //}, () =>
              //{
              //    ExeTExt();

              //}, () =>
              //{
              //    ExeTExt();

              //}, () =>
              //{
              //    ExeTExt();

              //}, () =>
              //{
              //    ExeTExt();

              //}, () =>
              //{
              //    ExeTExt();

              //}, () =>
              //{
              //    ExeTExt();

              //}, () =>
              //{
              //    ExeTExt();

              //}
              );

            

        }

        public void Dispose()
        {
            if (databasePool != null)
            {
                databasePool.Dispose();
            }

            //if (databaseWrapperPool != null)
            //{
            //    databaseWrapperPool.Dispose();
            //}
        }
    }







}
