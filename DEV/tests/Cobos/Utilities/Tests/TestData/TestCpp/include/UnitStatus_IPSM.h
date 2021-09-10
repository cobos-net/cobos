//-----------------------------------------------------------------------------------
// Filename:		UnitStatus_IPSM.h
//
// Overview: 
//		Declaration of CIPSM_UnitStatus class.
//
//-----------------------------------------------------------------------------------

#if !defined(AFX_UnitStatus_IPSM_H__ED7B69BF_6EAE_11D2_AE4B_0000F87A3173__INCLUDED_)
#define AFX_UnitStatus_IPSM_H__ED7B69BF_6EAE_11D2_AE4B_0000F87A3173__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

const int DISPATCH_STAFF_METHOD_FOLLOW_PRIMARY		=1;
const int DISPATCH_STAFF_METHOD_FOLLOW_APPLIANCE	=2;

#include <string>
#include <list>
#include <vector>
#include <memory>

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


//-----------------------------------------------------------------------------------
// Class: class CIPSM_UnitStatus  
//
// Abstract:
//		This is a singleton class to ensure that all parameter file calls go through
//		the one controlling critical section.
//
//		NOTE that the use of this class does not prevent you from calling
//		the global AuParameter calls yourself, but if you want to ensure threadsafety
//		you should always make the calls through this class.
//
// Written By:
//      Peter Soukalopoulos
//
// Modification History
//		Peter Soukalopoulos    	Initial version									29/10/98
//-----------------------------------------------------------------------------------
class CIPSM_UnitStatus  
{
public:
	void CloseParameterFile();
	static CIPSM_UnitStatus* Instance();
	static void KillInstance();
	bool InitOK() {return m_bInitOK;}

	#ifdef _AFXDLL
		CString GetTerminal();
	bool OpenTable(const CString sTableName);
	bool OpenAltStaff(const CString sTableName);
	CString GetMnemonic(CString &, const CString, const CString);
	CString GetMnemonic(CString &sResult, const long nParameter, const CString sDefault);
	CString GetColour(CString &, const CString, const CString);
	int GetMethod(int &, const CString, const int);
	long GetId(long &, const CString, const long);
	long GetRecommend(long &, const CString, const long);
	CString GetRecommendStatusList();
	CString GetRecommendStatusListByMnemonic();
	CString GetEventStatusList();
	CString GetEventStatusListByMnemonic();
	bool GetValueStatusDisplayList(CString sParamName, CString& sValue);
	bool IsStaffStation(const CString &sStation);
		bool	IsRecommendable(CString p_strUnitStatus);
		bool	IsOnEvent(CString p_strUnitStatus);
	#else
		// not using MFC
		std::string GetTerminal();
		std::string GetRecommendStatusList();
		std::string GetRecommendStatusListByMnemonic();
		std::string GetEventStatusList();
		std::string GetEventStatusListByMnemonic();
	#endif // _AFXDLL

	bool OpenTable(const char *p_szTableName);
	bool OpenAltStaff(const char *p_szTableName);
	std::string GetMnemonic(std::string &, const char *, const char *);
	std::string GetMnemonic(std::string &sResult, const long nParameter, const char *p_szDefault);
	std::string GetColour(std::string &, const char *, const char *);
	int GetMethod(int &, const char *, const int);
	long GetId(long &, const char *, const long);
	long GetRecommend(long &, const char *, const long);
	bool GetValueStatusDisplayList(const char *p_szParamName, std::string & sValue);
	bool IsStaffStation(const char *p_szStation);
	
	bool GetIdByMnemonic(long &nResult, const char *p_szMnemonic);
	
	
	
	

	std::list< std::string >	GetRecommendableMnemonicsList();
	std::list< long >			GetRecommendableStatusesList();

	std::list< std::string >	GetOnEventMnemonicsList();
	std::list< long >			GetOnEventStatusesList();

	/**
		For the following three status values, the p_szUnitStatuses string is a list of unit status values
		separated by any of the characters in the p_szDelimiters string.  The values can be any of
		 - mnemonics
		 - verbose unit statuses
		 - status integers.

		These methods simply breakup the incoming list of values and try to interpret them as any of these
		types of values.  Any values that cannot be identified as a unit status will be returned in the
		p_pslFailedStatuses list (if provided)
	 */
	std::list< std::string >	GetMnemonicsForStatuses( const char *p_szUnitStatuses, const char *p_szDelimiters = ", -\n\t", std::list< std::string > *p_pslFailedStatuses = NULL );
	std::list< std::string >	GetVerboseStatusesForStatuses( const char *p_szUnitStatuses, const char *p_szDelimiters = ", -\n\t", std::list< std::string > *p_pslFailedStatuses = NULL );
	std::list< long >			GetStatusIntegersForStatuses( const char *p_szUnitStatuses, const char *p_szDelimiters = ", -\n\t", std::list< std::string > *p_pslFailedStatuses = NULL );

	std::string		GetMnemonicForStatus( const char *p_szUnitStatus );
	std::string		GetMnemonicForStatus( const int p_iUnitStatus );
	long			GetStatusIntegerForStatus( const char *p_szUnitStatus );

	std::string		GetVerboseStatusForStatus( const char *p_szUnitStatus );
	std::string		GetVerboseStatusForStatus( const int p_iUnitStatus );

	bool	IsRecommendable(int p_iUnitStatus);
	bool	IsRecommendable(const char *p_szUnitStatus);
	
	bool	IsOnEvent(int p_iUnitStatus);
	bool	IsOnEvent(const char *p_szUnitStatus);
	

	std::string GetXML();

protected:
	CIPSM_UnitStatus();
	// virtual ~CIPSM_UnitStatus();
public:
	~CIPSM_UnitStatus();

private:
	// static CIPSM_UnitStatus* s_instance;

	static std::auto_ptr<CIPSM_UnitStatus> s_instance;
	static bool s_bUnitStatusTableAlreadyOpenned;

	CRITICAL_SECTION		m_csLock;
	bool m_bInitOK;
	std::vector<std::string>	m_lPFMnemonic;
	std::vector<std::string>	m_lPFColour;
	std::vector<std::string>	m_lPFStatus;

	std::vector<std::string>	m_lPFStation;
	std::vector<int>		m_lPFMethod;
	std::vector<long>		m_lPFId;
	std::vector<long>		m_lPFRecommend;
	std::vector<long>		m_lPFEvent;

	std::string m_ssRecStatuses;
	std::string m_ssRecStatusesMnemonic;
	std::string m_ssEventStatuses;
	std::string m_ssEventStatusesMnemonic;
	std::string m_ssTerminal;

private:
	class cAutoLock {
	public:
		cAutoLock( CRITICAL_SECTION *p_pCriticalSection ) {
			m_pCriticalSection = p_pCriticalSection;
			::EnterCriticalSection(m_pCriticalSection);
		} // End of constructor

		~cAutoLock() {
			::LeaveCriticalSection(m_pCriticalSection);
		} // End of destructor.

	private:
		CRITICAL_SECTION *m_pCriticalSection;

	}; // End of auto_lock template definition


};

#endif // !defined(AFX_UnitStatus_IPSM_H__ED7B69BF_6EAE_11D2_AE4B_0000F87A3173__INCLUDED_)
