using System;
using System.IO;

namespace Ajubaa.IBModeler.Common
{
    public class ProcessInfo
    {
        public bool g_blnCancelProcess = false;
        public float g_fltPercentComplete = 0.0f;
        public uint g_uintTotalCount = 0;
        public Stream MoldData { get; set; }
        //public Stream g_strOutputFile;
        public string g_strSecurity = "";


	    public void setSecurity(string inStrSecurity) 
        {
		    g_strSecurity = inStrSecurity;
	    }

        public string getSecurity() 
        {
		    return g_strSecurity;
	    }

        public void cancelProcess() 
        {
		    g_blnCancelProcess = true;
	    }

        public float getCompletedPercent() 
        {
		    return g_fltPercentComplete;
	    }

        public bool isCancelled() 
        {
		    return g_blnCancelProcess;
	    }

        public void initMaxNumber(uint inUintMaxNumber) 
        {
		    g_uintTotalCount = inUintMaxNumber;
	    }

        public void setNewPercent(uint inUintCompleted) 
        {
		    if(!(g_uintTotalCount > 0 && inUintCompleted <= g_uintTotalCount))
                throw new Exception("Invalid value passed to setNewPercent");
		    float fltPercentComplete = ((float)inUintCompleted/(float)g_uintTotalCount)*100.0f;
		    if(!(fltPercentComplete >= g_fltPercentComplete))
                throw new Exception("The calculation gave a percentage value above the complete percent.");
		    g_fltPercentComplete = fltPercentComplete;
	    }
    }
}
