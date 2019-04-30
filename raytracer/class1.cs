/*
* MATLAB Compiler: 6.3 (R2016b)
* Date: Sat Apr 20 11:11:42 2019
* Arguments: "-B" "macro_default" "-W" "dotnet:T,Class1,0.0,private,remote" "-T"
* "link:lib" "-d" "C:\Users\13573\Desktop\第二步计算\T\for_testing" "-v"
* "class{Class1:C:\Users\13573\Desktop\第二步计算\T.m}" 
*/
using System;
using System.Reflection;
using System.IO;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;
using IT;

#if SHARED
[assembly: System.Reflection.AssemblyKeyFile(@"")]
#endif

namespace T
{

    /// <summary>
    /// The Class1 class provides a CLS compliant, MWArray interface to the MATLAB
    /// functions contained in the files:
    /// <newpara></newpara>
    /// C:\Users\13573\Desktop\第二步计算\T.m
    /// </summary>
    /// <remarks>
    /// @Version 0.0
    /// </remarks>
    public class Class1 : MarshalByRefObject, IT.IClass1, IDisposable
    {
        #region Constructors

        /// <summary internal= "true">
        /// The static constructor instantiates and initializes the MATLAB Runtime instance.
        /// </summary>
        static Class1()
        {
            if (MWMCR.MCRAppInitialized)
            {
                try
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();

                    string ctfFilePath = assembly.Location;

                    int lastDelimiter = ctfFilePath.LastIndexOf(@"\");

                    ctfFilePath = ctfFilePath.Remove(lastDelimiter, (ctfFilePath.Length - lastDelimiter));

                    string ctfFileName = "T.ctf";

                    Stream embeddedCtfStream = null;

                    String[] resourceStrings = assembly.GetManifestResourceNames();

                    foreach (String name in resourceStrings)
                    {
                        if (name.Contains(ctfFileName))
                        {
                            embeddedCtfStream = assembly.GetManifestResourceStream(name);
                            break;
                        }
                    }
                    mcr = new MWMCR("",
                                   ctfFilePath, embeddedCtfStream, true);
                }
                catch (Exception ex)
                {
                    ex_ = new Exception("MWArray assembly failed to be initialized", ex);
                }
            }
            else
            {
                ex_ = new ApplicationException("MWArray assembly could not be initialized");
            }
        }


        /// <summary>
        /// Constructs a new instance of the Class1 class.
        /// </summary>
        public Class1()
        {
            if (ex_ != null)
            {
                throw ex_;
            }
        }


        #endregion Constructors

        #region Finalize

        /// <summary internal= "true">
        /// Class destructor called by the CLR garbage collector.
        /// </summary>
        ~Class1()
        {
            Dispose(false);
        }


        /// <summary>
        /// Frees the native resources associated with this object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }


        /// <summary internal= "true">
        /// Internal dispose function
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;

                if (disposing)
                {
                    // Free managed resources;
                }

                // Free native resources
            }
        }


        #endregion Finalize

        #region Methods

        /// <summary>
        /// Provides a single output, 0-input MWArrayinterface to the T MATLAB function.
        /// </summary>
        /// <remarks>
        /// M-Documentation:
        /// m为测站数，n为公共点
        /// </remarks>
        /// <returns>An MWArray containing the first output argument.</returns>
        ///
        public MWArray T()
        {
            return mcr.EvaluateFunction("T", new MWArray[] { });
        }


        /// <summary>
        /// Provides the standard 0-input MWArray interface to the T MATLAB function.
        /// </summary>
        /// <remarks>
        /// M-Documentation:
        /// m为测站数，n为公共点
        /// </remarks>
        /// <param name="numArgsOut">The number of output arguments to return.</param>
        /// <returns>An Array of length "numArgsOut" containing the output
        /// arguments.</returns>
        ///
        public MWArray[] T(int numArgsOut)
        {
            return mcr.EvaluateFunction(numArgsOut, "T", new MWArray[] { });
        }


        /// <summary>
        /// Provides an interface for the T function in which the input and output
        /// arguments are specified as an array of MWArrays.
        /// </summary>
        /// <remarks>
        /// This method will allocate and return by reference the output argument
        /// array.<newpara></newpara>
        /// M-Documentation:
        /// m为测站数，n为公共点
        /// </remarks>
        /// <param name="numArgsOut">The number of output arguments to return</param>
        /// <param name= "argsOut">Array of MWArray output arguments</param>
        /// <param name= "argsIn">Array of MWArray input arguments</param>
        ///
        public void T(int numArgsOut, ref MWArray[] argsOut, MWArray[] argsIn)
        {
            mcr.EvaluateFunction("T", numArgsOut, ref argsOut, argsIn);
        }



        /// <summary>
        /// This method will cause a MATLAB figure window to behave as a modal dialog box.
        /// The method will not return until all the figure windows associated with this
        /// component have been closed.
        /// </summary>
        /// <remarks>
        /// An application should only call this method when required to keep the
        /// MATLAB figure window from disappearing.  Other techniques, such as calling
        /// Console.ReadLine() from the application should be considered where
        /// possible.</remarks>
        ///
        public void WaitForFiguresToDie()
        {
            mcr.WaitForFiguresToDie();
        }



        #endregion Methods

        #region Class Members

        private static MWMCR mcr = null;

        private static Exception ex_ = null;

        private bool disposed = false;

        #endregion Class Members
    }
}
