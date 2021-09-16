//-----------------------------------------------------------------------------------
// Filename:		UnitStatus_IPSM.cpp
//
// Overview: 
//		Implementation of CIPSM_UnitStatus class.
//		Wrapper for parameter file to make them threadsafe.
//
//-----------------------------------------------------------------------------------

#pragma warning(disable:4786)

// #include "stdafx.h"

#ifdef _AFXDLL
	// We're using MFC
	#ifndef __AFX_H__
		#include <afx.h>	// For CRITICAL_SECTION definition.
	#endif // __AFX_H__
#else
	#ifndef _WINDOWS_
		#include <windows.h>
	#endif // _WINDOWS_
#endif // _AFXDLL

#include "cadversion.h"

#include "UnitStatus_IPSM.h"
#include "Parameter_IPSM.h"
#include "stdExtensions.h"		// For ips::BreakByDelimiters()

#include <algorithm>

#define CAD_PF_LENGTH	1024

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#endif

#include "IpsLog.h"


#include <sstream>
#include <string>
#include <vector>

// added for RCMerge testing
#include "dummy.h"
#include "include/dummy2.h"

//////////////////////////////////////////////////////////////////////
// Singleton Stuff
//////////////////////////////////////////////////////////////////////

// CIPSM_UnitStatus* CIPSM_UnitStatus::s_instance = NULL;
std::auto_ptr<CIPSM_UnitStatus> CIPSM_UnitStatus::s_instance;
bool CIPSM_UnitStatus::s_bUnitStatusTableAlreadyOpenned = false;


//-----------------------------------------------------------------------------------
// Method:		CIPSM_UnitStatus* CIPSM_UnitStatus::Instance()
//
// Abstract: 
//		Entry point, all access to this object comes through here.
//		Added initialisation of parameter file table
//
// Arguments:
//
// Returns:
//
// Written By:
//	    Peter Soukalopoulos
//
// Modification History
//		Peter Soukalopoulos		Initial version								29/11/98
//-----------------------------------------------------------------------------------
CIPSM_UnitStatus* CIPSM_UnitStatus::Instance()
{
	ips::log log("CIPSM_UnitStatus::Instance() ");

	try {
		if (NULL == s_instance.get()) {
			s_instance.reset(new CIPSM_UnitStatus);
		} // End if - then

		return(s_instance.get());
	} // End try
	catch(...) {
		log.fail() << "Exception caught\n";
		throw;	// Re-throw the exception.
	} // End catch
}

//-----------------------------------------------------------------------------------
// Method:		void CIPSM_UnitStatus::KillInstance()
//
// Abstract: 
//		Explicit cleanup of the object.  Call this when you're through with it.
//
// Arguments:
//
// Returns:
//
// Written By:
//	    Peter Soukalopoulos
//
// Modification History
//		Peter Soukalopoulos		Initial version								29/11/98
//-----------------------------------------------------------------------------------
void CIPSM_UnitStatus::KillInstance()
{
	s_instance.reset();
}


	
//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

//-----------------------------------------------------------------------------------
// Method:		CIPSM_UnitStatus::CIPSM_UnitStatus()
//
// Abstract: 
//		Constructor.  Initialises critical section and reads CDTS info from parameter file.
//		Sets the terminal name as the lowercase version of the environment string "COMPUTERNAME".
//
// Arguments:
//
// Returns:
//
// Written By:
//	    Peter Soukalopoulos
//
// Modification History
//		Peter Soukalopoulos		Initial version								29/11/98
//		Peter Soukalopoulos		Updated for CAD 7.4							11/1/00
//-----------------------------------------------------------------------------------
CIPSM_UnitStatus::CIPSM_UnitStatus()
{

	ips::log log("CIPSM_UnitStatus::CIPSM_UnitStatus() ");

	try {
		
		TCHAR szCAD_PF[CAD_PF_LENGTH];
		bool	nResult=false;

		InitializeCriticalSection(&m_csLock);

		cAutoLock	lock(&m_csLock);
		
		memset( szCAD_PF, '\0', sizeof(szCAD_PF) );

		#ifndef CAD_LOADABLE_MODULE
			auParameterInit();
		#else
			int l_iBufSize = sizeof(szCAD_PF);
			CDget_param( "CAD_PF", szCAD_PF, &l_iBufSize );
			nResult = true;	// Indicate that the auParameterInit() routine (which we don't need to call from a loadable module) is OK.
		#endif
#if defined _CADVERSION_ && CADVERSION >= 0x070905
		std::string	l_ssError;
		if(::auParameterSetState("cad", szCAD_PF, &s_bUnitStatusTableAlreadyOpenned, &l_ssError)) {
			nResult=true;
		} // End if - then
#else
		if(::auParameterSetState("cad", szCAD_PF)) {
			nResult=true;
		} // End if - then
#endif				
		m_ssTerminal = getenv("COMPUTERNAME");
		std::transform( m_ssTerminal.begin(), m_ssTerminal.end(), m_ssTerminal.begin(), tolower );

		m_bInitOK = nResult;
		if (! OpenTable("UnitStatus")) {
			log.fail() << "Could not open 'UnitStatus' table\n";
		} // End if - then
		else {
			s_bUnitStatusTableAlreadyOpenned = true;
		} // End if - else
	} // End try
	catch(...) {
		log.fail() << "Exception caught\n";
		throw;
	} // End catch
}

//-----------------------------------------------------------------------------------
// Method:		CIPSM_UnitStatus::~CIPSM_UnitStatus()
//
// Abstract: 
//		Destructor.
//
// Arguments:
//
// Returns:
//
// Written By:
//	    Peter Soukalopoulos
//
// Modification History
//		Peter Soukalopoulos		Initial version								29/11/98
//-----------------------------------------------------------------------------------
CIPSM_UnitStatus::~CIPSM_UnitStatus()
{
	ips::log log("CIPSM_UnitStatus::~CIPSM_UnitStatus() ");

	try {
		m_lPFStatus.clear();
		m_lPFMnemonic.clear();
		m_lPFColour.clear();
		m_lPFId.clear();
		m_lPFRecommend.clear();
		
		m_lPFStation.clear();
		m_lPFMethod.clear();

		s_bUnitStatusTableAlreadyOpenned = false;

		#ifndef CAD_LOADABLE_MODULE
			// Close the parameter file
			if (false == s_bUnitStatusTableAlreadyOpenned)
			{
				::auParameterClose(0);	
			}
		#endif // not a CAD_LOADABLE_MODULE
				
		DeleteCriticalSection(&m_csLock);
	} // End try
	catch(const std::exception &e)
	{
		log.fail() << "std exception : " << e.what() << "\n";
	}
	catch(...) {
		log.fail() << "Exception caught\n";
	} // End catch
}


//////////////////////////////////////////////////////////////////////
// The parameter file calls
//////////////////////////////////////////////////////////////////////

//-----------------------------------------------------------------------------------
// Method:		CString CIPSM_UnitStatus::OpenTable(const CString sTableName)
//
// Abstract: 
//		Reads specific UnitStatus columns from the named parameter file table into vector arrays.
//		Constructs a CSV string of recommend statuses.
//		Reads until the end of table without determining its size.
//
// Arguments:
//		IN: parameter table name; since the named vectors are from the "UnitStatus" table
//			this is the only supported tablename.
//
// Returns:
//		True if at least one (complete) row can be read from the table and the recommend
//		status string(s) are constructed, false otherwise.
//
// Written By:
//	    Peter Soukalopoulos
//
// Modification History
//		Peter Soukalopoulos		Initial version								29/11/98
//		Peter Soukalopoulos		Enhanced error logging on components		5/3/99
//		Peter Soukalopoulos		Added m_sEventStatus/Mnemonic list			22/3/99
//		Doug Moore				Added m_sRecStatususesMnemonic				22/3/99
//		Peter Soukalopoulos		Removed warning message from last row		9/4/99
//		Doug Moore				Added call to get number of rows in table	
//								Previously, there was no way to tell the			
//								difference to between an error and the last
//								row read. This should helps error reporting 8/1/02
//-----------------------------------------------------------------------------------

#ifdef _AFXDLL
	bool CIPSM_UnitStatus::OpenTable(const CString sTableName) {
		return(this->OpenTable( (LPCSTR) sTableName ));
	} // End OpenTable() method
#endif

bool CIPSM_UnitStatus::OpenTable(const char *p_szTableName) {

	ips::log log("CIPSM_UnitStatus::OpenTable() ");

	const bool l_bSuccess = true;
	const bool l_bFailure = false;

	try {
		char	szTable[1024];
		unsigned int			i = 0;
		int			iRetCode=1;

		double	nValue=0;
		int		nType=0;
		int		nNumRows = 0;
		std::string strErrorMsg = "";

		CIPSM_UnitStatus::cAutoLock	lock(&m_csLock);

		if (s_bUnitStatusTableAlreadyOpenned) {
			// We don't want to load it twice.  Just return ok.
			return(l_bSuccess);
		} // End if - then

		strcpy(szTable,p_szTableName);

		log.debug(10) << "'" << szTable << "' parameter file table\n";

		iRetCode = ::auTableGetNumRows(szTable, &nNumRows, &strErrorMsg);
		if (!iRetCode)
		{
			// failed in get the number of rows in table
			log.fail() << "failed to retrieve the number of rows in table [" << szTable << "] error msg [" << strErrorMsg.c_str() << "]\n";
			return(l_bFailure);
		}
		else if (nNumRows == 0)
		{
			// zero rows in table?
			log.fail() << "found zero rows in table [" << szTable << "]; no statuses to read?\n";
			return(l_bFailure);
		}
		else
		{
			m_lPFEvent.clear();
			m_lPFStatus.clear();
			m_lPFMnemonic.clear();
			m_lPFColour.clear();
			m_lPFId.clear();
			m_lPFRecommend.clear();

		
			// while more rows and no errors continue reading rows
			for (i=0; i < (unsigned int) nNumRows; ++i) 
			{
				try {
					m_lPFStatus.push_back(::auTableGetColumn(szTable,"Status",i).c_str());
					m_lPFMnemonic.push_back(::auTableGetColumn(szTable,"Mnemonic",i).c_str());
					m_lPFColour.push_back(::auTableGetColumn(szTable,"Color",i).c_str());
					m_lPFId.push_back(atoi(::auTableGetColumn(szTable,"ID",i).c_str()));
					m_lPFRecommend.push_back(atoi(::auTableGetColumn(szTable,"Recommendable",i).c_str()));
					m_lPFEvent.push_back(atoi(::auTableGetColumn(szTable,"Event",i).c_str()));
				} // End try
				catch(const std::exception & error) {
					log.fail() << error.what() << "\n";
					m_lPFStatus.clear();
					m_lPFMnemonic.clear();
					m_lPFColour.clear();
					m_lPFId.clear();
					m_lPFRecommend.clear();
					m_lPFEvent.clear();
					return(l_bFailure);
				} // End catch
			}	// end for (all rows)
			
			if (i < (unsigned int) nNumRows || !iRetCode)
			{
				// failed to read all rows?
				log.fail() << "Error occured while reading rows from parameter table [" << szTable << "]\n";
				return(l_bFailure);
			}

			m_ssRecStatuses.erase();	
			m_ssRecStatusesMnemonic.erase();	
			m_ssEventStatuses.erase();
			m_ssEventStatusesMnemonic.erase();

			for (i=0;i<m_lPFRecommend.size() ;i++) 
			{
				if (m_lPFRecommend[i]==1) 
				{
					long	nId;
					std::stringstream ssStatus;
					std::stringstream sMnemonic;

					nId=m_lPFId[i];
					ssStatus << nId << ", ";

					sMnemonic << m_lPFMnemonic[i];

					m_ssRecStatuses += ssStatus.str();
					m_ssRecStatusesMnemonic += "\"" + sMnemonic.str() + "\", ";
				}
			}

			// Remove trailing comma
			if (m_ssRecStatuses.size() > 0) {
				m_ssRecStatuses = m_ssRecStatuses.substr(0,m_ssRecStatuses.size()-2);
			} // End if - then
			else {
				log.warn() << "Could not build RecStatuses string.\n";
			} // End if - else

			if (m_ssRecStatusesMnemonic.size() > 0) {
				m_ssRecStatusesMnemonic = m_ssRecStatusesMnemonic.substr(0,m_ssRecStatusesMnemonic.size()-2);
			} // End if - then
			else {
				log.warn() << "Could not build RecStatusesMnemonic string.\n";
			} // End if - else

			for (i=0;i<m_lPFEvent.size() ;i++) 
			{
				if (m_lPFEvent[i]==1) 
				{
					long	nId;
					std::stringstream ossStatus;
					std::stringstream ossMnemonic;

					nId=m_lPFId[i];
					ossStatus << nId << ", ";
					
					ossMnemonic << m_lPFMnemonic[i];

					m_ssEventStatuses += ossStatus.str();
					m_ssEventStatusesMnemonic += "\"" + ossMnemonic.str() + "\", ";
				}
			}

			// Remove trailing comma
			if (m_ssEventStatuses.size() > 0) {
				m_ssEventStatuses = m_ssEventStatuses.substr(0,m_ssEventStatuses.size()-2);
			} // End if - then
			else {
				log.warn() << "Could not build EventStatuses string.\n";
			} // End if - else

			if (m_ssEventStatusesMnemonic.size() > 0) {
				m_ssEventStatusesMnemonic = m_ssEventStatusesMnemonic.substr(0, m_ssEventStatusesMnemonic.size()-2);
			} // End if - then
			else {
				log.warn() << "Could not build EventStatusesMnemonic string.\n";
			} // End if - else
		}

		return(m_lPFStatus.size()>0 && 
			(m_ssRecStatuses.size() > 0) && 
			(m_ssEventStatuses.size() > 0) &&
			(m_ssEventStatusesMnemonic.size() > 0) &&
			(m_ssRecStatusesMnemonic.size() > 0)
			);
	} // End try
	catch(const std::exception &e)
	{
		log.fail() << "std exception : " << e.what() << "\n";
		return(l_bFailure);
	}
	catch(...) {
		log.fail() << "Exception caught\n";
		throw;
	} // End catch
}


//-----------------------------------------------------------------------------------
// Method:		GetMnemonic(CString &sResult, const CString sParameter, const CString sDefault)
//
// Abstract: 
//		Returns the mnemonic for the status parameter
//
// Arguments:
//			 const CString sParameter: name of status paramter
//			 const CString sDefault: default to return if not found
//		OUT: CString &sResult: mnemonic for named parameter or default if not found
//
// Returns:
//		The mnemonic or the default if not found.
//
// Written By:
//	    Peter Soukalopoulos
//
// Modification History
//		Peter Soukalopoulos		Initial version								29/11/98
//-----------------------------------------------------------------------------------

#ifdef _AFXDLL
	CString CIPSM_UnitStatus::GetMnemonic(CString &sResult, const CString sParameter, const CString sDefault) {
		std::string l_ssResult = (LPCSTR) sResult;

		this->GetMnemonic( l_ssResult, (LPCSTR) sParameter, (LPCSTR) sDefault );

		sResult = l_ssResult.c_str();
		return(CString(l_ssResult.c_str()));
	} // End GetMnemonic() method
#endif // _AFXDLL

std::string CIPSM_UnitStatus::GetMnemonic(std::string &sResult, const char *p_szParameter, const char *p_szDefault) {
	ips::log log("CIPSM_UnitStatus::GetMnemonic() ");
	unsigned int nIdx;
	bool bFound=false;

	sResult = p_szDefault;

	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);

	for (nIdx=0;nIdx<m_lPFStatus.size() && !bFound;nIdx++) {
		if (m_lPFStatus[nIdx] == std::string(p_szParameter))
			bFound=true;
	}

	if (bFound)
		sResult=m_lPFMnemonic[--nIdx];
	else
		log.warn() << "'" << (LPCSTR) p_szParameter << "' parameter default '" << (LPCSTR) p_szDefault << "' used\n";
	
	return (sResult);

}


//-----------------------------------------------------------------------------------
// Method:		GetMnemonic(CString &sResult, const long nParameter, const CString sDefault)
//
// Abstract: 
//		Returns the mnemonic for the status parameter
//
// Arguments:
//			 const short nParameter: id of status paramter
//			 const CString sDefault: default to return if not found
//		OUT: CString &sResult: mnemonic for named parameter or default if not found
//
// Returns:
//		The mnemonic or the default if not found.
//
// Written By:
//	    Peter Soukalopoulos
//
// Modification History
//		Peter Soukalopoulos		Initial version								11/1/00
//-----------------------------------------------------------------------------------

#ifdef _AFXDLL
	CString CIPSM_UnitStatus::GetMnemonic(CString &sResult, const long nParameter, const CString sDefault) {
		std::string l_ssResult = sResult;
		this->GetMnemonic( l_ssResult, nParameter, (LPCSTR) sDefault );
		sResult = l_ssResult.c_str();
		return(sResult);
	} // End GetMnemonic() method
#endif // _AFXDLL

std::string CIPSM_UnitStatus::GetMnemonic(std::string &sResult, const long nParameter, const char *p_szDefault) {

	ips::log log("CIPSM_UnitStatus::GetMnemonic() ");
	unsigned int nIdx;
	bool bFound=false;

	sResult=p_szDefault;

	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);

	for (nIdx=0;nIdx<m_lPFId.size() && !bFound;nIdx++) {
		if (m_lPFId[nIdx]==nParameter)
			bFound=true;
	}

	if (bFound)
		sResult = m_lPFMnemonic[--nIdx];
	else
		log.warn() << "'" << nParameter << "' parameter default '" << (LPCSTR) p_szDefault << "' used\n";
	
	return (sResult);

}


//-----------------------------------------------------------------------------------
// Method:		GetColour(CString &sResult, const CString sParameter, const CString sDefault)
//
// Abstract: 
//		Returns the colour for the status parameter
//
// Arguments:
//			 const CString sParameter: name of status paramter
//			 const CString sDefault: default to return if not found
//		OUT: CString &sResult: colour for named parameter or default if not found
//
// Returns:
//		The colour or the default if not found.
//
// Written By:
//	    Peter Soukalopoulos
//
// Modification History
//		Peter Soukalopoulos		Initial version								29/11/98
//-----------------------------------------------------------------------------------

#ifdef _AFXDLL
	CString CIPSM_UnitStatus::GetColour(CString &sResult, const CString sParameter, const CString sDefault) {
		std::string l_ssResult = sResult;
		this->GetColour( l_ssResult, (LPCSTR) sParameter, (LPCSTR) sDefault );
		sResult = l_ssResult.c_str();
		return(sResult);
	} // End GetColour() method
#endif // _AFXDLL

std::string CIPSM_UnitStatus::GetColour(std::string &sResult, const char *p_szParameter, const char *p_szDefault) {

	ips::log log("CIPSM_UnitStatus::GetColour() ");
	unsigned int nIdx;
	bool bFound=false;

	sResult=p_szDefault;

	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);

	for (nIdx=0;nIdx<m_lPFStatus.size() && !bFound;nIdx++) {
		if (m_lPFStatus[nIdx]== std::string(p_szParameter))
			bFound=true;
	}

	if (bFound)
		sResult=m_lPFColour[--nIdx];
	else
		log.warn() << "'" << (LPCSTR) p_szParameter << "' parameter default '" << (LPCSTR) p_szDefault << "' used\n";

	return (sResult);

}


//-----------------------------------------------------------------------------------
// Method:		GetIdGetId(long &nResult, const CString sParameter, const long nDefault)
//
// Abstract: 
//		Returns the Id for the status parameter
//
// Arguments:
//			 const CString sParameter: name of status paramter
//			 const long nDefault: default to return if not found
//		OUT: long &nResult: id for named parameter or default if not found
//
// Returns:
//		The id or the default if not found.
//
// Written By:
//	    Peter Soukalopoulos
//
// Modification History
//		Peter Soukalopoulos		Initial version								29/11/98
//-----------------------------------------------------------------------------------

#ifdef _AFXDLL
	long CIPSM_UnitStatus::GetId(long &nResult, const CString sParameter, const long nDefault) {
		return( this->GetId( nResult, (LPCSTR) sParameter, nDefault ));
	} // End GetId() method
#endif // _AFXDLL

long CIPSM_UnitStatus::GetId(long &nResult, const char *p_szParameter, const long nDefault) {

	ips::log log("CIPSM_UnitStatus::GetId() ");
	unsigned int nIdx;
	bool bFound=false;

	nResult=nDefault;

	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);

	for (nIdx=0;nIdx<m_lPFStatus.size() && !bFound;nIdx++) {
		if (m_lPFStatus[nIdx]==std::string(p_szParameter))
			bFound=true;
	}

	if (bFound)
		nResult=m_lPFId[--nIdx];
	else
		log.warn() << "'" << (LPCSTR) p_szParameter << "' parameter default '" << nDefault << "' used\n";

	return (nResult);

}


//-----------------------------------------------------------------------------------
// Method:		GetIdGetId(long &nResult, const CString sParameter, const long nDefault)
//
// Abstract: 
//		Returns the Id for the status parameter
//
// Arguments:
//			 const char *mnemonic
//		OUT: long &nResult: id for specified mnemonic
//
// Returns:
//		true on success, false otherwise
//
// Written By:
//	    David Nicholls
//
// Modification History
//		David Nicholls			Initial version							24-JUL-2002
//-----------------------------------------------------------------------------------

bool CIPSM_UnitStatus::GetIdByMnemonic(long &nResult, const char *p_szMnemonic) {

	const bool l_bSuccess = true;
	const bool l_bFailure = false;

	ips::log log("CIPSM_UnitStatus::GetIdByMnemonic() ");

	unsigned int nIdx;
	std::string l_ssMnemonic = p_szMnemonic;

	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);

	for (nIdx=0;nIdx<m_lPFMnemonic.size(); nIdx++) {
		if (m_lPFMnemonic[nIdx]==l_ssMnemonic) {
			nResult = m_lPFId[nIdx];
			return(l_bSuccess);
		} // End if - then
	}

	log.warn() << "'" << p_szMnemonic << "' Could not find mnemonic\n";
	return(l_bFailure);

}

//-----------------------------------------------------------------------------------
// Method:		GetRecommend(long &nResult, const CString sParameter, const long nDefault)
//
// Abstract: 
//		Returns the recommend status for the status parameter
//
// Arguments:
//			 const CString sParameter: name of status paramter
//			 const long nDefault: default to return if not found
//		OUT: long &nResult: recommend status for named parameter or default if not found
//
// Returns:
//		The recommend status or the default if not found.
//
// Written By:
//	    Peter Soukalopoulos
//
// Modification History
//		Peter Soukalopoulos		Initial version								29/11/98
//-----------------------------------------------------------------------------------

#ifdef _AFXDLL
	long CIPSM_UnitStatus::GetRecommend(long &nResult, const CString sParameter, const long nDefault) {
		return(this->GetRecommend( nResult, (LPCSTR) sParameter, nDefault ));
	} // End GetRecommend() method
#endif // _AFXDLL

long CIPSM_UnitStatus::GetRecommend(long &nResult, const char *p_szParameter, const long nDefault) {

	ips::log log("CIPSM_UnitStatus::GetRecommend() ");
	unsigned int nIdx;
	bool bFound=false;

	nResult=nDefault;

	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);

	for (nIdx=0;nIdx<m_lPFStatus.size() && !bFound;nIdx++) {
		if (m_lPFStatus[nIdx]==std::string(p_szParameter))
			bFound=true;
	}

	if (bFound)
		nResult=m_lPFRecommend[--nIdx];
	else
		log.warn() << "'" << (LPCSTR) p_szParameter << "' parameter default '" << nDefault << "' used\n";

	return (nResult);

}

//-----------------------------------------------------------------------------------
// Method:		GetMethod(int &iResult, const CString sParameter, const int iDefault)
//
// Abstract: 
//		Returns the method for the station parameter
//
// Arguments:
//			 const CString sStation: name of station
//			 const int iDefault: default to return if not found
//		OUT: int &iResult: method for named station or default if not found
//
// Returns:
//		The method or the default if not found.
//
// Written By:
//	    Peter Soukalopoulos
//
// Modification History
//		Peter Soukalopoulos		Initial version								29/11/98
//-----------------------------------------------------------------------------------

#ifdef _AFXDLL
	int CIPSM_UnitStatus::GetMethod(int &iResult, const CString sStation, const int iDefault) {
		return(this->GetMethod( iResult, (LPCSTR) sStation, iDefault ));
	} // End GetMethod() method
#endif // _AFXDLL

int CIPSM_UnitStatus::GetMethod(int &iResult, const char *p_szStation, const int iDefault) {

	ips::log log("CIPSM_UnitStatus::GetMethod() ");
	unsigned int nIdx;
	bool bFound=false;

	iResult=iDefault;

	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);

	for (nIdx=0;nIdx<m_lPFStation.size() && !bFound;nIdx++) {
		if (m_lPFStation[nIdx]== std::string(p_szStation))
			bFound=true;
	}

	if (bFound)
		iResult=m_lPFMethod[--nIdx];
	else
		log.warn() << "'" << (LPCSTR) p_szStation << "' station default (" << iDefault << ") used\n";

	return (iResult);

}

//-----------------------------------------------------------------------------------
// Method:		IsStaffStation(const CString sStation)
//
// Abstract: 
//		Determines if a given station is a full time staff station
//
// Arguments:
//			 const CString sStation: name of station
//
// Returns:
//		true if the station is a full time staff station, false otherwise.
//
// Written By:
//	    Steven Lynch
//
// Modification History
//		Steven Lynch		Initial version								6/10/99
//-----------------------------------------------------------------------------------
#ifdef _AFXDLL
	bool CIPSM_UnitStatus::IsStaffStation(const CString &sStation) {
		return(this->IsStaffStation( (LPCSTR) sStation ));
	} // End IsStaffStation() method
#endif // _AFXDLL

bool CIPSM_UnitStatus::IsStaffStation(const char *p_szStation)
{
	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);

	unsigned int nIdx = 0;
	for (; nIdx < m_lPFStation.size(); nIdx++) 
	{
		if (m_lPFStation[nIdx] == std::string(p_szStation))		break;
	}
	return ((nIdx < m_lPFStation.size()) && 
			((m_lPFMethod[nIdx] == DISPATCH_STAFF_METHOD_FOLLOW_PRIMARY) ||
			 (m_lPFMethod[nIdx] == DISPATCH_STAFF_METHOD_FOLLOW_APPLIANCE)));
}

//-----------------------------------------------------------------------------------
// Method:		CString CIPSM_UnitStatus::OpenAltStaff(const CString sTableName)
//
// Abstract: 
//		Reads Alt Staff columns from the named parameter file table into vector arrays
//
// Arguments:
//		IN: parameter table name; since the named vectors are from the "OpenAltStaff" table
//			this is the only supported tablename.
//
// Returns:
//		True if at least one (complete) row can be read from the table, false otherwise.
//
// Written By:
//	    Peter Soukalopoulos
//
// Modification History
//		Peter Soukalopoulos		Initial version								29/11/98
//		Peter Soukalopoulos		Enhanced error logging on components		5/3/99
//-----------------------------------------------------------------------------------
#ifdef _AFXDLL
	bool CIPSM_UnitStatus::OpenAltStaff(const CString sTableName) {
		return(this->OpenAltStaff( (LPCSTR) sTableName ));
	} // End OpenAltStaff() method
#endif // _AFXDLL

bool CIPSM_UnitStatus::OpenAltStaff(const char *p_szTableName)
{
	ips::log log("CIPSM_UnitStatus::OpenAltStaff() ");
	const bool l_bSuccess = true;
	const bool l_bFailure = false;

	int			nNumRows=0;
	std::string l_ssErrorMsg;

	log.debug(10) << "'" << (LPCSTR) p_szTableName << "' parameter file table\n";

	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);

	if (! ::auTableGetNumRows( (char *) p_szTableName, &nNumRows, &l_ssErrorMsg))
	{
		// failed in get the number of rows in table
		log.fail() << "failed to retrieve the number of rows in table [" << p_szTableName << "] error msg [" << l_ssErrorMsg.c_str() << "]\n";
		return(l_bFailure);
	}
	
	if (nNumRows == 0)
	{
		// zero rows in table?
		log.fail() << "found zero rows in table [" << p_szTableName << "]; no statuses to read?\n";
		return(l_bFailure);
	}


	for (int l_iRow=0; l_iRow < nNumRows; ++l_iRow) {
		std::string l_ssStation, l_ssMethod;

		try {
			m_lPFStation.push_back(::auTableGetColumn((char *) p_szTableName,"Station",l_iRow).c_str());
		} // End try
		catch(const std::exception & error) {
			(void) error.what();	// Avoid the compiler warning.
			log.warn() << "Could not retrieve Station for row " << l_iRow << " - OK if last row\n";
			break;
		} // End catch

		try {
			m_lPFMethod.push_back(atoi(::auTableGetColumn((char *) p_szTableName,"Method",l_iRow).c_str()));
		} // End try
		catch(const std::exception & error) {
			(void) error.what();	// Avoid the compiler warning.
			log.warn() << "Could not retrieve Method for row " << l_iRow << "\n";
			break;
		} // End catch
	} // End for

	log.init() << "read " << m_lPFStation.size() << " rows from '" << p_szTableName << "' parameter file table\n";

	return(m_lPFStation.size()>0);
}


//-----------------------------------------------------------------------------------
// Method:	bool CIPSM_UnitStatus::GetValueStatusDisplayList
//			(
//				const CString sParamName,
//				CString& strValue
//			)
//
// Abstract: 
//		Reads the value for the specified parameter in the StatusDisplay list 
//
// Arguments:
//		IN:		parameter name within the StatusDisplay list
//		OUT:	value retrievef from parameter
//
// Returns:
//		BOOL	TRUE  => value read successfully
//				FALSE => failed to read value
//
// Written By:
//		Doug Moore (used Peter S. OpenAltStaff() method as a template)
//
// Modification History
//
//-----------------------------------------------------------------------------------

#ifdef _AFXDLL
	bool CIPSM_UnitStatus::GetValueStatusDisplayList
	(
		CString	sParamName,
		CString&		sValue
		) {
		std::string l_ssValue = sValue;
		bool l_bStatus = this->GetValueStatusDisplayList( (LPCSTR) sParamName, l_ssValue );
		sValue = l_ssValue.c_str();
		return(l_bStatus);
	} // End GetValueStatusDisplayList() method
#endif // _AFXDLL

bool CIPSM_UnitStatus::GetValueStatusDisplayList
(
	const char *p_szParamName,
	std::string&		sValue
)
{
	ips::log log("CIPSM_UnitStatus::GetValueStatusDisplayList() ");
	bool		status = true;

	log.debug(10) << "getting value for parameter " << p_szParamName << "\n";		

	try
	{
		CIPSM_UnitStatus::cAutoLock lock(&m_csLock);

		int type = 0;
		char value[256];
		memset(value, 0, sizeof(value));

		int iRetCode = (::auParameterGet("StatusDisplay", (char *) p_szParamName,
						&type, (void *)value, sizeof(value)));
		if (1 == iRetCode)
		{
			sValue = (char *)value;
		}
		else
		{
			status = false;
			log.warn() << "Could not retrieve value for parameter " << p_szParamName << " from StatusDisplay list\n";
		}
	}
	catch(const std::exception &e)
	{
		log.fail() << "std exception : " <<  e.what() << "\n";
		status = false;
	}
	catch(...)
	{
		log.fail() << "Exception thrown!\n";
		throw;
	}

	return(status);
}

//-----------------------------------------------------------------------------------
// Method:		CString CIPSM_UnitStatus::GetTerminal()
//
// Abstract:	Returns the terminal name
//
// Arguments:	None
//
// Returns:		The workstation COMPUTERNAME
//
// Written By:
//	    Peter Soukalopoulos
//
// Modification History
//		Peter Soukalopoulos		Initial version								5/3/99
//-----------------------------------------------------------------------------------

#ifdef _AFXDLL
	CString CIPSM_UnitStatus::GetTerminal()
	{
		CIPSM_UnitStatus::cAutoLock lock(&m_csLock);
		return(CString(m_ssTerminal.c_str()));

	}
#else
	std::string CIPSM_UnitStatus::GetTerminal()
	{
	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);
		return(m_ssTerminal);

	}
#endif // _AFXDLL

//-----------------------------------------------------------------------------------
// Method:		CString CIPSM_UnitStatus::GetRecommendStatusList()
//
// Abstract:	Returns the previously constructed CSV CString of recommend statuses
//
// Arguments:	None
//
// Returns:		CSV CString of recommend statuses
//
// Written By:
//	    Peter Soukalopoulos
//
// Modification History
//		Peter Soukalopoulos		Initial version								5/3/99
//-----------------------------------------------------------------------------------
#ifdef _AFXDLL
	CString CIPSM_UnitStatus::GetRecommendStatusList() {

	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);
		return(CString(m_ssRecStatuses.c_str()));
	}
#else
	std::string CIPSM_UnitStatus::GetRecommendStatusList() {

		CIPSM_UnitStatus::cAutoLock lock(&m_csLock);
		return(m_ssRecStatuses);
	}
#endif // _AFXDLL

//-----------------------------------------------------------------------------------
// Method:		CString CIPSM_UnitStatus::GetRecommendStatusListByMnemonic()
//
// Abstract:	Returns the previously constructed CSV CString of recommend statuses
//				by mnemonic
//
// Arguments:	None
//
// Returns:		CSV CString of recommend statuses
//
// Written By:
//	    Doug Moore	
//
// Modification History
//		Doug Moore				Initial version	adapted from Peter H's		22/3/99
//								GetEventStatusListByMnemonic()
//-----------------------------------------------------------------------------------
#ifdef _AFXDLL
	CString CIPSM_UnitStatus::GetRecommendStatusListByMnemonic() {

	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);
		return(CString(m_ssRecStatusesMnemonic.c_str()));
	}
#else
	std::string CIPSM_UnitStatus::GetRecommendStatusListByMnemonic() {

		CIPSM_UnitStatus::cAutoLock lock(&m_csLock);
		return(m_ssRecStatusesMnemonic);
	}

#endif // _AFXDLL

//-----------------------------------------------------------------------------------
// Method:		CString CIPSM_UnitStatus::GetEventStatusList()
//
// Abstract:	Returns the previously constructed CSV CString of event statuses
//
// Arguments:	None
//
// Returns:		CSV CString of event statuses
//
// Written By:
//	    Peter Soukalopoulos
//
// Modification History
//		Peter Soukalopoulos		Initial version								12/3/99
//-----------------------------------------------------------------------------------
#ifdef _AFXDLL
	CString CIPSM_UnitStatus::GetEventStatusList() {

	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);
		return(CString(m_ssEventStatuses.c_str()));
	}
#else
	std::string CIPSM_UnitStatus::GetEventStatusList() {

		CIPSM_UnitStatus::cAutoLock lock(&m_csLock);
		return(m_ssEventStatuses);
	}
#endif // #ifdef _AFXDLL


//-----------------------------------------------------------------------------------
// Method:		CString CIPSM_UnitStatus::GetEventStatusListByMnemonic()
//
// Abstract:	Returns the previously constructed CSV CString of event statuses by 
///				mnemonic
//
// Arguments:	None
//
// Returns:		CSV CString of event statuses by mnemonic
///
// Written By:
//	    Peter Soukalopoulos
//
// Modification History
//		Peter Soukalopoulos		Initial version								22/3/99
//-----------------------------------------------------------------------------------
#ifdef _AFXDLL
	CString CIPSM_UnitStatus::GetEventStatusListByMnemonic()
	{
		CIPSM_UnitStatus::cAutoLock lock(&m_csLock);
		return(CString(m_ssEventStatusesMnemonic.c_str()));
	}
#else
	std::string CIPSM_UnitStatus::GetEventStatusListByMnemonic()
	{
	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);
		return(m_ssEventStatusesMnemonic);
	}

#endif // _AFXDLL

//-----------------------------------------------------------------------------------
// Method:		void CIPSM_UnitStatus::CloseParameterFile()
//
// Abstract:	Closes the CAD parameter file
//
// Arguments:	None
//
// Returns:		None
//
// Written By:
//	    Peter Soukalopoulos
//
// Modification History
//		Peter Soukalopoulos		Initial version								5/3/99
//-----------------------------------------------------------------------------------
void CIPSM_UnitStatus::CloseParameterFile()
{

	::auParameterClose(0);	

}

bool
CIPSM_UnitStatus::IsRecommendable(int p_iUnitStatus)
{
	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);

	unsigned int i = 0;
	for (;i<m_lPFId.size(); i++)
	{
		if (m_lPFId[i] == p_iUnitStatus)
			break;
	}

	if (i>= m_lPFId.size())
		return false;

	if (m_lPFRecommend[i])
		return true;
	return false;
}

#ifdef _AFXDLL
	bool
	CIPSM_UnitStatus::IsRecommendable(CString p_strUnitStatus)
	{
		return(this->IsRecommendable( (LPCSTR) p_strUnitStatus ));
	}
#endif // _AFXDLL

bool
CIPSM_UnitStatus::IsRecommendable(const char *p_szUnitStatus)
{
	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);

	unsigned int i = 0;
	for (;i<m_lPFMnemonic.size(); i++)
	{
		if (m_lPFMnemonic[i] == std::string(p_szUnitStatus))
			break;
	}

	if (i>= m_lPFId.size())
		return false;

	if (m_lPFRecommend[i])
		return true;
	return false;
}

bool
CIPSM_UnitStatus::IsOnEvent(int p_iUnitStatus)
{
	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);

	unsigned int i=0;
	for (;i<m_lPFId.size(); i++)
	{
		if (m_lPFId[i] == p_iUnitStatus)
			break;
	}

	if (i>= m_lPFId.size())
		return false;

	if (m_lPFEvent[i])
		return true;
	return false;
}


#ifdef _AFXDLL
bool
CIPSM_UnitStatus::IsOnEvent(CString p_strUnitStatus)
{
	return(this->IsOnEvent( (LPCSTR) p_strUnitStatus ));
}
#endif // _AFXDLL

bool
CIPSM_UnitStatus::IsOnEvent(const char *p_szUnitStatus)
{
	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);

	unsigned int i=0;
	for (;i<m_lPFMnemonic.size(); i++)
	{
		if (m_lPFMnemonic[i] == std::string(p_szUnitStatus))
			break;
	}

	if (i>= m_lPFId.size())
		return false;

	if (m_lPFEvent[i])
		return true;
	return false;
}


std::list< std::string >
CIPSM_UnitStatus::GetRecommendableMnemonicsList() 
{

	std::list< std::string >	l_slRecommendableMnemonics;

	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);

	for (unsigned int i=0;i<m_lPFRecommend.size() ;i++) 
	{
		if (m_lPFRecommend[i] == 1) 
		{
			l_slRecommendableMnemonics.push_back( m_lPFMnemonic[i].c_str() );
		}
	}

	return(l_slRecommendableMnemonics);

} // End of GetRecommendableMnemonicsList() method.


std::list< long >
CIPSM_UnitStatus::GetRecommendableStatusesList() 
{

	std::list< long >	l_slRecommendableStatuses;

	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);

	for (unsigned int i=0;i<m_lPFRecommend.size() ;i++) 
	{
		if (m_lPFRecommend[i] == 1) 
		{
			l_slRecommendableStatuses.push_back( (long) m_lPFId[i] );
		}
	}

	return(l_slRecommendableStatuses);

} // End of GetRecommendableStatusesList() method.



std::list< std::string >
CIPSM_UnitStatus::GetOnEventMnemonicsList() 
{

	std::list< std::string >	l_slOnEventMnemonics;

	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);

	for (unsigned int i=0;i<m_lPFEvent.size() ;i++) 
	{
		if (m_lPFEvent[i]) 
		{
			l_slOnEventMnemonics.push_back( m_lPFMnemonic[i] );
		}
	}

	return(l_slOnEventMnemonics);

} // End of GetOnEventMnemonicsList() method.


std::list< long >
CIPSM_UnitStatus::GetOnEventStatusesList() 
{

	std::list< long >	l_slOnEventStatuses;

	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);

	for (unsigned int i=0;i<m_lPFEvent.size() ;i++) 
	{
		if (m_lPFEvent[i] == 1) 
		{
			l_slOnEventStatuses.push_back( (long) m_lPFId[i] );
		}
	}

	return(l_slOnEventStatuses);

} // End of GetOnEventStatusesList() method.



std::string CIPSM_UnitStatus::GetXML()
{ 
	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);
	std::ostringstream l_ossXml;

	l_ossXml << "<UNIT_STATUS_TABLE>\n";

	for (unsigned int i=0; i<m_lPFMnemonic.size(); i++) {
		l_ossXml << "\t<ENTRY>\n";
		l_ossXml << "\t\t<MNEMONIC>" << m_lPFMnemonic[i].c_str() << "</MNEMONIC>\n";
		l_ossXml << "\t\t<COLOUR>" << m_lPFColour[i].c_str() << "</COLOUR>\n";
		l_ossXml << "\t\t<STATUS>" << m_lPFStatus[i].c_str() << "</STATUS>\n";
		l_ossXml << "\t\t<ID>" << m_lPFId[i] << "</ID>\n";
		l_ossXml << "\t\t<RECOMMEND>" << m_lPFRecommend[i] << "</RECOMMEND>\n";
		l_ossXml << "\t\t<EVENT>" << m_lPFEvent[i] << "</EVENT>\n";
		l_ossXml << "\t</ENTRY>\n";
	} // End for

	l_ossXml << "</UNIT_STATUS_TABLE>\n";

	return(l_ossXml.str());

} // End of GetXML() method



/**
	This method takes a unit status in any of the three valid forms (verbose, mnemonic or integer)
	and returns the corresponding mnemonic string.  If the status cannot be found, an empty string is
	returned.
 */
std::string	CIPSM_UnitStatus::GetMnemonicForStatus( const char *p_szUnitStatus )
{

	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);
	std::string l_ssStatusToFind = p_szUnitStatus;

	std::transform( l_ssStatusToFind.begin(), l_ssStatusToFind.end(), l_ssStatusToFind.begin(), toupper );

	unsigned int i=0;
	for (;i<m_lPFMnemonic.size(); i++)
	{
		std::string l_ssUpper = ips::upper( m_lPFMnemonic[i].c_str() );

		if (l_ssUpper == l_ssStatusToFind) {
			return(m_lPFMnemonic[i] );
		} // End if - then				
	} // End for

	// It's not a mnemonic.  See if it's a verbose status instead.

	for (i=0;i<m_lPFStatus.size(); i++)
	{
		std::string l_ssUpper = ips::upper( m_lPFStatus[i].c_str() );

		if (l_ssUpper == l_ssStatusToFind) {
			return(m_lPFMnemonic[i]);
		} // End if - then				
	} // End for

	// It's not a verbose status.  See if it's an integer.

	if (ips::AllNumeric( l_ssStatusToFind.c_str() )) {
		unsigned long l_iStatus = strtoul( l_ssStatusToFind.c_str(), NULL, 10 );
		if ((l_iStatus >= 0) && (l_iStatus < m_lPFStatus.size())) {
			return(m_lPFMnemonic[l_iStatus]);
		} // End if - then
	} // End if - then

	return(std::string(""));

} // End of GetMnemonicForStatus() method


std::string	CIPSM_UnitStatus::GetMnemonicForStatus( const int p_iUnitStatus )
{

	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);
	
	if ((p_iUnitStatus >= 0) && (p_iUnitStatus < (int) m_lPFStatus.size())) {
		return(m_lPFMnemonic[p_iUnitStatus]);
	} // End if - then
	
	return(std::string(""));

} // End of GetMnemonicForStatus() method




std::list< std::string >	CIPSM_UnitStatus::GetMnemonicsForStatuses( 
	const char *p_szUnitStatuses, 
	const char *p_szDelimiters, /* = ", -\n\t" */
	std::list< std::string > *p_pslFailedStatuses /* = NULL */ )
{

	std::list< std::string >	l_slResult;

	std::vector<std::string> l_svStatuses = ips::BreakByDelimiters( p_szUnitStatuses, p_szDelimiters );
	std::vector<std::string>::iterator l_itStatus;

	if (p_pslFailedStatuses) {
		p_pslFailedStatuses->erase(p_pslFailedStatuses->begin(), p_pslFailedStatuses->end());
	} // End if - then

	for (l_itStatus = l_svStatuses.begin(); l_itStatus != l_svStatuses.end(); l_itStatus++) {
		bool l_bFound = false;

		std::string l_ssStatusToFind = *l_itStatus;

		std::transform( l_ssStatusToFind.begin(), l_ssStatusToFind.end(), l_ssStatusToFind.begin(), toupper );

		CIPSM_UnitStatus::cAutoLock lock(&m_csLock);

		for (unsigned int i=0;i<m_lPFMnemonic.size(); i++)
		{
			std::string l_ssUpper = ips::upper( m_lPFMnemonic[i].c_str() );

			if (l_ssUpper == l_ssStatusToFind) {
				l_slResult.push_back( m_lPFMnemonic[i] );
				l_bFound = true;
			} // End if - then				
		} // End for

		if (l_bFound == false) {
			// It's not a mnemonic.  See if it's a verbose status instead.

			for (unsigned int i=0;i<m_lPFStatus.size(); i++)
			{
				std::string l_ssUpper = ips::upper( m_lPFStatus[i].c_str() );

				if (l_ssUpper == l_ssStatusToFind) {
					l_slResult.push_back( m_lPFMnemonic[i] );
					l_bFound = true;
				} // End if - then				
			} // End for
		} // End if - then

		if (l_bFound == false) {
			// It's not a verbose status.  See if it's an integer.

			if (ips::AllNumeric( l_itStatus->c_str() )) {
				unsigned long l_iStatus = strtoul( l_itStatus->c_str(), NULL, 10 );
				if ((l_iStatus >= 0) && (l_iStatus < m_lPFStatus.size())) {
					l_slResult.push_back( m_lPFMnemonic[l_iStatus] );
					l_bFound = true;
				} // End if - then
			} // End if - then
		} // End if - then

		if (l_bFound == false) {
			if (p_pslFailedStatuses) {
				p_pslFailedStatuses->push_back( *l_itStatus );
			} // End if - then
		} // End if - then
	} // End for

	return(l_slResult);

} // End of GetMnemonicsForStatuses() method.




std::list< std::string >	CIPSM_UnitStatus::GetVerboseStatusesForStatuses( 
	const char *p_szUnitStatuses, 
	const char *p_szDelimiters, /* = ", -\n\t" */
	std::list< std::string > *p_pslFailedStatuses /* = NULL */ )
{

	std::list< std::string >	l_slResult;

	std::vector<std::string> l_svStatuses = ips::BreakByDelimiters( p_szUnitStatuses, p_szDelimiters );
	std::vector<std::string>::iterator l_itStatus;

	if (p_pslFailedStatuses) {
		p_pslFailedStatuses->erase(p_pslFailedStatuses->begin(), p_pslFailedStatuses->end());
	} // End if - then

	for (l_itStatus = l_svStatuses.begin(); l_itStatus != l_svStatuses.end(); l_itStatus++) {
		bool l_bFound = false;
		std::string l_ssStatusToFind = *l_itStatus;

		std::transform( l_ssStatusToFind.begin(), l_ssStatusToFind.end(), l_ssStatusToFind.begin(), toupper );

		CIPSM_UnitStatus::cAutoLock lock(&m_csLock);

		for (unsigned int i=0;i<m_lPFMnemonic.size(); i++)
		{
			std::string l_ssUpper = ips::upper( m_lPFMnemonic[i].c_str() );

			if (l_ssUpper == l_ssStatusToFind) {
				l_slResult.push_back( m_lPFStatus[i] );
				l_bFound = true;
			} // End if - then				
		} // End for

		if (l_bFound == false) {
			// It's not a mnemonic.  See if it's a verbose status instead.

			for (unsigned int i=0;i<m_lPFStatus.size(); i++)
			{
				std::string l_ssUpper = ips::upper( m_lPFStatus[i].c_str() );

				if (l_ssUpper == l_ssStatusToFind) {
					l_slResult.push_back( m_lPFStatus[i] );
					l_bFound = true;
				} // End if - then				
			} // End for
		} // End if - then

		if (l_bFound == false) {
			// It's not a verbose status.  See if it's an integer.

			if (ips::AllNumeric( l_itStatus->c_str() )) {
				unsigned long l_iStatus = strtoul( l_itStatus->c_str(), NULL, 10 );
				if ((l_iStatus >= 0) && (l_iStatus < m_lPFStatus.size())) {
					l_slResult.push_back( m_lPFStatus[l_iStatus] );
					l_bFound = true;
				} // End if - then
			} // End if - then
		} // End if - then

		if (l_bFound == false) {
			if (p_pslFailedStatuses) {
				p_pslFailedStatuses->push_back( *l_itStatus );
			} // End if - then
		} // End if - then
	} // End for

	return(l_slResult);

} // End of GetVerboseStatusesForStatuses() method.


std::string	CIPSM_UnitStatus::GetVerboseStatusForStatus( const char *p_szUnitStatus )
{
	std::string l_ssStatusToFind = p_szUnitStatus;

	std::transform( l_ssStatusToFind.begin(), l_ssStatusToFind.end(), l_ssStatusToFind.begin(), toupper );

	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);

	for (unsigned int i=0;i<m_lPFMnemonic.size(); i++)
	{
		std::string l_ssUpper = ips::upper( m_lPFMnemonic[i].c_str() );

		if (l_ssUpper == l_ssStatusToFind) {
			return( m_lPFStatus[i] );
		} // End if - then				
	} // End for

	// It's not a mnemonic.  See if it's a verbose status instead.

	for (unsigned int i=0;i<m_lPFStatus.size(); i++)
	{
		std::string l_ssUpper = ips::upper( m_lPFStatus[i].c_str() );

		if (l_ssUpper == l_ssStatusToFind) {
			return( m_lPFStatus[i] );
		} // End if - then				
	} // End for


	// It's not a verbose status.  See if it's an integer.

	if (ips::AllNumeric( l_ssStatusToFind.c_str() )) {
		unsigned long l_iStatus = strtoul( l_ssStatusToFind.c_str(), NULL, 10 );
		if ((l_iStatus >= 0) && (l_iStatus < m_lPFStatus.size())) {
			return( m_lPFStatus[l_iStatus] );
		} // End if - then
	} // End if - then


	// It hasn't been found.
	return("");

} // End GetVerboseStatusForStatus() method



std::string	CIPSM_UnitStatus::GetVerboseStatusForStatus( const int p_iUnitStatus )
{
	std::ostringstream l_ossStatus;
	l_ossStatus << p_iUnitStatus;
	return(GetVerboseStatusForStatus(l_ossStatus.str().c_str()));

} // End GetVerboseStatusForStatus() method


long CIPSM_UnitStatus::GetStatusIntegerForStatus( const char *p_szUnitStatus )
{

	std::list< long >	l_slResult;
	
	std::string l_ssStatusToFind = p_szUnitStatus;

	std::transform( l_ssStatusToFind.begin(), l_ssStatusToFind.end(), l_ssStatusToFind.begin(), toupper );

	CIPSM_UnitStatus::cAutoLock lock(&m_csLock);

	unsigned int i=0;
	for (;i<m_lPFMnemonic.size(); i++)
	{
		std::string l_ssUpper = ips::upper( m_lPFMnemonic[i].c_str() );

		if (l_ssUpper == l_ssStatusToFind) {
			return( m_lPFId[i] );
		} // End if - then				
	} // End for

	// It's not a mnemonic.  See if it's a verbose status instead.
	for (i=0;i<m_lPFStatus.size(); i++)
	{
		std::string l_ssUpper = ips::upper( m_lPFStatus[i].c_str() );

		if (l_ssUpper == l_ssStatusToFind) {
			return( m_lPFId[i] );
		} // End if - then				
	} // End for

	
	// It's not a verbose status.  See if it's an integer.

	if (ips::AllNumeric( p_szUnitStatus )) {
		unsigned long l_iStatus = strtoul( p_szUnitStatus, NULL, 10 );
		if ((l_iStatus >= 0) && (l_iStatus < m_lPFStatus.size())) {
			return( long(l_iStatus) );
		} // End if - then
	} // End if - then

	return(-1);

} // End of GetStatusIntegerForStatus() method



std::list< long >	CIPSM_UnitStatus::GetStatusIntegersForStatuses( 
	const char *p_szUnitStatuses, 
	const char *p_szDelimiters, /* = ", -\n\t" */
	std::list< std::string > *p_pslFailedStatuses /* = NULL */ )
{

	std::list< long >	l_slResult;

	std::vector<std::string> l_svStatuses = ips::BreakByDelimiters( p_szUnitStatuses, p_szDelimiters );
	std::vector<std::string>::iterator l_itStatus;

	if (p_pslFailedStatuses) {
		p_pslFailedStatuses->erase(p_pslFailedStatuses->begin(), p_pslFailedStatuses->end());
	} // End if - then

	for (l_itStatus = l_svStatuses.begin(); l_itStatus != l_svStatuses.end(); l_itStatus++) {
		bool l_bFound = false;
		std::string l_ssStatusToFind = *l_itStatus;

		std::transform( l_ssStatusToFind.begin(), l_ssStatusToFind.end(), l_ssStatusToFind.begin(), toupper );

		CIPSM_UnitStatus::cAutoLock lock(&m_csLock);

		for (unsigned int i=0;i<m_lPFMnemonic.size(); i++)
		{
			std::string l_ssUpper = ips::upper( m_lPFMnemonic[i].c_str() );
			if (l_ssUpper == l_ssStatusToFind) {
				l_slResult.push_back( m_lPFId[i] );
				l_bFound = true;
			} // End if - then				
		} // End for

		if (l_bFound == false) {
			// It's not a mnemonic.  See if it's a verbose status instead.

			for (unsigned int i=0;i<m_lPFStatus.size(); i++)
			{
				std::string l_ssUpper = ips::upper( m_lPFStatus[i].c_str() );
				if (l_ssUpper == l_ssStatusToFind) {
					l_slResult.push_back( m_lPFId[i] );
					l_bFound = true;
				} // End if - then				
			} // End for
		} // End if - then

		if (l_bFound == false) {
			// It's not a verbose status.  See if it's an integer.

			if (ips::AllNumeric( l_itStatus->c_str() )) {
				unsigned long l_iStatus = strtoul( l_itStatus->c_str(), NULL, 10 );
				if ((l_iStatus >= 0) && (l_iStatus < m_lPFStatus.size())) {
					l_slResult.push_back( long(l_iStatus) );
					l_bFound = true;
				} // End if - then
			} // End if - then
		} // End if - then

		if (l_bFound == false) {
			if (p_pslFailedStatuses) {
				p_pslFailedStatuses->push_back( *l_itStatus );
			} // End if - then
		} // End if - then
	} // End for

	return(l_slResult);

} // End of GetStatusIntegersForStatuses() method.






