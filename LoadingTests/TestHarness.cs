/////////////////////////////////////////////////////////////////////
// TestHarness.cs - Runs tests by loading dlls and invoking test() //
//                                                                 //
// RAHUL VIJAYDEV, CSE681 - Software Modeling and Analysis, Fall 2016 //
/////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace LoadingTests
{
  class TestHarness
  {
    private struct TestData
    {
      public string Name;
      public ITest testDriver;
    }

    private List<TestData> testDriver = new List<TestData>();

    TestHarness() { }

    //----< load test dlls to invoke >-------------------------------

    bool LoadTests(string path)
    {
      try
      {
        string[] files = System.IO.Directory.GetFiles(path, "*.dll");

        
          Console.Write("\n  loading: \"{0}\"", files[0]);

          Assembly assem = Assembly.LoadFrom(files[0]);
          Type[] types = assem.GetExportedTypes();

          foreach (Type t in types)
          {
            if (t.IsClass && typeof(ITest).IsAssignableFrom(t))  // does this type derive from ITest ?
            {
              ITest tdr = (ITest)Activator.CreateInstance(t);    // create instance of test driver

              // save type name and reference to created type on managed heap

              TestData td = new TestData();
              td.Name = t.Name;
              td.testDriver = tdr;
              testDriver.Add(td);
            }
          }
        
        Console.Write("\n");
      }
      catch(Exception ex)
      {
        Console.Write("\n\n  {0}\n\n", ex.Message);
        return false;
      }
      return testDriver.Count > 0;   // if we have items in list then Load succeeded
    }
    //----< run all the tests on list made in LoadTests >------------

    void run()
    {
      if (testDriver.Count == 0)
        return;
      foreach (TestData td in testDriver)  // enumerate the test list
      {
        Console.Write("\n  testing {0}", td.Name);
        if (td.testDriver.test() == true)
          Console.Write("\n  test passed");
        else
          Console.Write("\n  test failed");
      }
    }
    static void Main(string[] args)
    {
      // using string path = "../../../Tests/TestDriver.dll" from command line;

      if (args.Count() == 0)
      {
        Console.Write("\n  Please enter path to libraries on command line\n\n");
        return;
      }
      string path = args[0];

      TestHarness th = new TestHarness();
      if (th.LoadTests(path))
        th.run();
      else
        Console.Write("\n  couldn't load tests");

      Console.Write("\n\n");
    }
  }
}
