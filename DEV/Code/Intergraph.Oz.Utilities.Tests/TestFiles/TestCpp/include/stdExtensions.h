
#ifndef STD_EXTENSIONS_TEST_FIXTURE_HEADER
#define STD_EXTENSIONS_TEST_FIXTURE_HEADER

#include "StdExtensions.h"
#include <cppunit/extensions/HelperMacros.h>





namespace ips { namespace stdextensions {

#pragma once

class test : public CPPUNIT_NS::TestFixture
{
  CPPUNIT_TEST_SUITE( ips::stdextensions::test );
  CPPUNIT_TEST( Format );
  CPPUNIT_TEST( sprintf );
  CPPUNIT_TEST( Affirmative );
  CPPUNIT_TEST( strupr );
  CPPUNIT_TEST( strlwr );
  CPPUNIT_TEST( upper );
  CPPUNIT_TEST( lower );
  CPPUNIT_TEST( QuotedOrNull );
  CPPUNIT_TEST( ltrim );
  CPPUNIT_TEST( rtrim );
  CPPUNIT_TEST( BreakByDelimiters1 );
  CPPUNIT_TEST( BreakByDelimiters2 );
  CPPUNIT_TEST( BreakByDelimiters3 );
  CPPUNIT_TEST( BreakByDelimiters4 );
  CPPUNIT_TEST( ParseIntoFields1 );
  CPPUNIT_TEST( ParseIntoFields2 );
  CPPUNIT_TEST( ReplaceDelimiters );
  CPPUNIT_TEST( FixQuotes );
  CPPUNIT_TEST( XmlDataStream );
  CPPUNIT_TEST( FromXmlData );
  CPPUNIT_TEST( PrintableString1 );
  CPPUNIT_TEST( PrintableString2 );
  CPPUNIT_TEST( ReplacePattern );
  CPPUNIT_TEST( ReplaceTextInstance );
  CPPUNIT_TEST( ReplaceTextBetween );
  CPPUNIT_TEST( AllNumeric );
  CPPUNIT_TEST( AllAlphaNumeric );
  CPPUNIT_TEST_SUITE_END();

public:
  void setUp()
  {
  }

public:
  void Format()
  {
	  CPPUNIT_ASSERT_MESSAGE( "ips::Format() test failed", ips::Format("David is a %s bloke", "good") == "David is a good bloke" );
  }


  void sprintf()
  {
	  std::string l_ssResult;
	  int l_iLengthOfResult = ips::sprintf( l_ssResult, "David is a %s bloke", "good" );

	  CPPUNIT_ASSERT_MESSAGE( "ips::sprintf() num fields wrong", l_iLengthOfResult == l_ssResult.size() );
	  CPPUNIT_ASSERT_MESSAGE( "ips::sprintf() resultant string wrong", l_ssResult == "David is a good bloke" );
  }

  void Affirmative()
  {
		CPPUNIT_ASSERT_MESSAGE( "ips::Affirmative(YES) is incorrect", ips::Affirmative("YES") == true );
		CPPUNIT_ASSERT_MESSAGE( "ips::Affirmative(YES) is incorrect", ips::Affirmative("Yes") == true );
		CPPUNIT_ASSERT_MESSAGE( "ips::Affirmative(YES) is incorrect", ips::Affirmative("yes") == true );

		CPPUNIT_ASSERT_MESSAGE( "ips::Affirmative(Y) is incorrect", ips::Affirmative("Y") == true );
		CPPUNIT_ASSERT_MESSAGE( "ips::Affirmative(Y) is incorrect", ips::Affirmative("y") == true );

		CPPUNIT_ASSERT_MESSAGE( "ips::Affirmative(TRUE) is incorrect", ips::Affirmative("TRUE") == true );
		CPPUNIT_ASSERT_MESSAGE( "ips::Affirmative(TRUE) is incorrect", ips::Affirmative("True") == true );
		CPPUNIT_ASSERT_MESSAGE( "ips::Affirmative(TRUE) is incorrect", ips::Affirmative("true") == true );

		CPPUNIT_ASSERT_MESSAGE( "ips::Affirmative(T) is incorrect", ips::Affirmative("T") == true );
		CPPUNIT_ASSERT_MESSAGE( "ips::Affirmative(T) is incorrect", ips::Affirmative("t") == true );

		CPPUNIT_ASSERT_MESSAGE( "ips::Affirmative(OK) is incorrect", ips::Affirmative("OK") == true );
		CPPUNIT_ASSERT_MESSAGE( "ips::Affirmative(OK) is incorrect", ips::Affirmative("Ok") == true );
		CPPUNIT_ASSERT_MESSAGE( "ips::Affirmative(OK) is incorrect", ips::Affirmative("ok") == true );

		CPPUNIT_ASSERT_MESSAGE( "ips::Affirmative(NO) is incorrect", ips::Affirmative("NO") == false );
  }
  

  void strupr()
  {
	
	  std::string l_ssValue = "Abc";
      CPPUNIT_ASSERT_MESSAGE( "ips::strupr(Abc) is incorrect", (strcmp(ips::strupr(l_ssValue), "ABC") == 0) );
  }

  void strlwr()
  {
	
	  std::string l_ssValue = "Abc";
      CPPUNIT_ASSERT_MESSAGE( "ips::strlwr(Abc) is incorrect", (strcmp(ips::strlwr(l_ssValue), "abc") == 0) );
  }

  void upper()
  {
	  CPPUNIT_ASSERT_MESSAGE( "ips::upper(Abc) is incorrect", ips::upper("Abc") == std::string("ABC") );
  }

  void lower()
  {
	  CPPUNIT_ASSERT_MESSAGE( "ips::lower(Abc) is incorrect", ips::lower("Abc") == std::string("abc") );
  }

  void QuotedOrNull()
  {
	CPPUNIT_ASSERT_MESSAGE( "ips::QuotedOrNull(\"\", true) is incorrect", ips::QuotedOrNull("", true) == std::string("IS NULL") );
	CPPUNIT_ASSERT_MESSAGE( "ips::QuotedOrNull(\"\", false) is incorrect", ips::QuotedOrNull("", false) == std::string("NULL") );

	CPPUNIT_ASSERT_MESSAGE( "ips::QuotedOrNull(\"\", false) is incorrect", ips::QuotedOrNull("42", true) == std::string("='42'") );
	CPPUNIT_ASSERT_MESSAGE( "ips::QuotedOrNull(\"\", false) is incorrect", ips::QuotedOrNull("42", false) == std::string("'42'") );
  }

  void ltrim()
  {
	  std::string value = "   something   ";
	  CPPUNIT_ASSERT_MESSAGE( "ips::ltrim(\"     something   \", \" \") is incorrect", ips::ltrim(value, " ") == std::string("something   ") );
  }

  void rtrim()
  {
	  std::string value = "   something   ";
	  CPPUNIT_ASSERT_MESSAGE( "ips::rtrim(\"     something   \", \" \") is incorrect", ips::rtrim(value, " ") == std::string("   something") );
  }

  void BreakByDelimiters1()
  {
	  std::string l_ssValue = "one/two\\three four;=five";
	  std::vector<std::string> l_svResult;
	  l_svResult.push_back("one");
	  l_svResult.push_back("two");
	  l_svResult.push_back("three");
	  l_svResult.push_back("four");
	  l_svResult.push_back("five");
	  
	  CPPUNIT_ASSERT_MESSAGE( "ips::BreakByDelimiters1() is incorrect", ips::BreakByDelimiters( l_ssValue.c_str(), " \\/;=" ) == l_svResult );
  }

  void BreakByDelimiters2()
  {
	  std::string l_ssValue = "one/two\\three four;=five";
	  std::vector<std::string> l_svResult;
	  l_svResult.push_back("one");
	  l_svResult.push_back("two");
	  l_svResult.push_back("thr");	l_svResult.push_back("ee");
	  l_svResult.push_back("fou");	l_svResult.push_back("r");
	  l_svResult.push_back("fiv");	l_svResult.push_back("e");

	  std::vector<std::string> fred = ips::BreakByDelimiters( "abcdefghijklmnopq", 3, " \\/;=" );
	  for (std::vector<std::string>::size_type i=0; i<fred.size(); i++)
	  {
		  char hello[1024];
		  strncpy( hello, fred[i].c_str(), sizeof(hello)-1 );
	  }
	  
	  CPPUNIT_ASSERT_MESSAGE( "ips::BreakByDelimiters2() is incorrect", ips::BreakByDelimiters( l_ssValue.c_str(), 3, " \\/;=" ) == l_svResult );
  }

  void BreakByDelimiters3()
  {
	  std::string l_ssValue = "one/two\\three four;=five";
	  std::list<std::string> l_slResult;
	  std::list<std::string> l_slExpected;
	  l_slExpected.push_back("one");
	  l_slExpected.push_back("two"); 
	  l_slExpected.push_back("three");
	  l_slExpected.push_back("four");
	  l_slExpected.push_back("five");
	  
	  ips::BreakByDelimiters( l_ssValue.c_str(), " \\/;=", &l_slResult );
	  CPPUNIT_ASSERT_MESSAGE( "ips::BreakByDelimiters3() is incorrect", l_slResult == l_slExpected );
  }

  void BreakByDelimiters4()
  {
	  std::string l_ssValue = "one/two\\three four;=five";
	  
	  std::vector<std::string> l_svExpected;
	  l_svExpected.push_back("one");
	  l_svExpected.push_back("two"); 
	  l_svExpected.push_back("three");
	  l_svExpected.push_back("four");
	  l_svExpected.push_back("five");
	  
	  std::vector<std::string>::size_type i=0;
	  ips::Tokens_t tokens = ips::BreakByDelimiters( l_ssValue.begin(), l_ssValue.end(), " \\/;=" );

	  CPPUNIT_ASSERT_MESSAGE( "ips::BreakByDelimiters4() gave wrong number of tokens", tokens.size() == l_svExpected.size() );

	  std::vector<std::string>::const_iterator l_itExpected;
	  ips::Tokens_t::const_iterator l_itToken;

	  for (l_itToken = tokens.begin(), l_itExpected = l_svExpected.begin(); 
		  
		  l_itToken != tokens.end() && l_itExpected != l_svExpected.end();
	  
		  l_itToken++, l_itExpected++)
	  {
		  std::string l_ssToken( l_itToken->first, l_itToken->second );

		  CPPUNIT_ASSERT_MESSAGE( "ips::BreakByDelimiters4() is incorrect", (l_ssToken == *l_itExpected ) );
	  } // End for
  }


  void ParseIntoFields1()
  {
	  std::vector< std::string > l_svFields = ips::ParseIntoFields( "one,\"two, and a bit\",,four\\\'s,five,,,,nine", ",", "\"'", false, "\\" );
	  std::vector< std::string > l_svExpected;

	  l_svExpected.push_back("one");
	  l_svExpected.push_back("two, and a bit");
	  l_svExpected.push_back("");
	  l_svExpected.push_back("four's");
	  l_svExpected.push_back("five");
	  l_svExpected.push_back("");
	  l_svExpected.push_back("");
	  l_svExpected.push_back("");
	  l_svExpected.push_back("nine");
	  
	  std::vector< std::string >::size_type lhs = l_svFields.size();
	  std::vector< std::string >::size_type rhs = l_svExpected.size();

	  CPPUNIT_ASSERT_MESSAGE("ParseIntoFields() failed", l_svFields.size() == l_svExpected.size());

	  std::vector<std::string>::const_iterator l_itField;
	  std::vector<std::string>::const_iterator l_itExpectedField;
	  for (l_itField = l_svFields.begin(), l_itExpectedField = l_svExpected.begin(); 
		   l_itField != l_svFields.end() && l_itExpectedField != l_svExpected.end(); 
		   l_itField++, l_itExpectedField++)
	  {
		  std::string fred = *l_itField;
		  CPPUNIT_ASSERT_MESSAGE("ParseIntoFields() failed", *l_itField == *l_itExpectedField );
	  }

  }

  void ParseIntoFields2()
  {
	  std::vector< std::string > l_svFields = ips::ParseIntoFields( "one,\"two, and a bit\",,four\\\'s,five,,,,nine", ",", "\"'", true, "\\" );
	  std::vector< std::string > l_svExpected;

	  l_svExpected.push_back("one");
	  l_svExpected.push_back("two, and a bit");
	  l_svExpected.push_back("four's");
	  l_svExpected.push_back("five");
	  l_svExpected.push_back("nine");
	  
	  std::vector< std::string >::size_type lhs = l_svFields.size();
	  std::vector< std::string >::size_type rhs = l_svExpected.size();

	  CPPUNIT_ASSERT_MESSAGE("ParseIntoFields() failed", l_svFields.size() == l_svExpected.size());

	  std::vector<std::string>::const_iterator l_itField;
	  std::vector<std::string>::const_iterator l_itExpectedField;
	  for (l_itField = l_svFields.begin(), l_itExpectedField = l_svExpected.begin(); 
		   l_itField != l_svFields.end() && l_itExpectedField != l_svExpected.end(); 
		   l_itField++, l_itExpectedField++)
	  {
		  std::string fred = *l_itField;
		  CPPUNIT_ASSERT_MESSAGE("ParseIntoFields() failed", *l_itField == *l_itExpectedField );
	  }

  }

  void ReplaceDelimiters()
  {
	  CPPUNIT_ASSERT_MESSAGE( "ips::ReplaceDelimiters() test failed", 
						ips::ReplaceDelimiters( "The tallest brown fox's house", "'", "QUOTE" ) == std::string("The tallest brown foxQUOTEs house"));
  }

  void FixQuotes()
  {
	  CPPUNIT_ASSERT_MESSAGE( "ips::FixQuotes() test failed", 
						ips::FixQuotes( "The tallest brown fox's house" ) == std::string("The tallest brown fox''s house"));
  }
  

  void XmlDataStream()
  {
	  CPPUNIT_ASSERT_MESSAGE( "ips::XmlDataStream() test failed", 
						ips::XmlDataStream( "Value with greater (>) less (<) ampersand (&) and apostrophe (') and quote (\") in it" ) == 
											std::string("Value with greater (&gt;) less (&lt;) ampersand (&amp;) and apostrophe (&apos;) and quote (&quot;) in it"));
  }

  void FromXmlData()
  {
	  CPPUNIT_ASSERT_MESSAGE( "ips::FromXmlData() test failed",
						ips::FromXmlData( "Value with greater (&gt;) less (&lt;) ampersand (&amp;) and apostrophe (&apos;) and quote (&quot;) in it" ) == 
											std::string("Value with greater (>) less (<) ampersand (&) and apostrophe (') and quote (\") in it"));
  }

  void PrintableString1()
  {
	  unsigned char l_szString[12];
	  memset( l_szString, '\0', sizeof(l_szString) );

	  l_szString[0] = '\r';
	  l_szString[1] = '\n';
	  l_szString[2] = '\033';
	  l_szString[3] = '\025';
	  l_szString[4] = '\006';
	  l_szString[5] = '\002';
	  l_szString[6] = '\003';
	  l_szString[7] = '\004';
	  l_szString[8] = 16;
	  l_szString[9] = 17;
	  l_szString[10] = 19;
	  l_szString[11] = 0x00;

	  CPPUNIT_ASSERT_MESSAGE( "ips::PrintableString1() test failed",
						ips::PrintableString( l_szString, sizeof(l_szString) ) == 
											std::string("<CR><LF><ESC><NAK><ACK><STX><ETX><EOT><DLE><XON><XOFF><NULL>"));
  }


  void PrintableString2()
  {
	  std::vector<unsigned char> l_svString;

	  l_svString.push_back('\r');
	  l_svString.push_back('\n');
	  l_svString.push_back('\033');
	  l_svString.push_back('\025');
	  l_svString.push_back('\006');
	  l_svString.push_back('\002');
	  l_svString.push_back('\003');
	  l_svString.push_back('\004');
	  l_svString.push_back(16);
	  l_svString.push_back(17);
	  l_svString.push_back(19);
	  l_svString.push_back(0x00);

	  CPPUNIT_ASSERT_MESSAGE( "ips::PrintableString2() test failed",
						ips::PrintableString( l_svString ) == 
											std::string("<CR><LF><ESC><NAK><ACK><STX><ETX><EOT><DLE><XON><XOFF><NULL>"));
  }

  void ReplacePattern()
  {
	  unsigned int l_iNumReplacements = 0;
	  std::string l_ssResult = ips::ReplacePattern( "one two three four five", "e", "E", &l_iNumReplacements );

	  CPPUNIT_ASSERT_MESSAGE( "ips::ReplacePattern1() test failed", l_iNumReplacements == 4);

	  CPPUNIT_ASSERT_MESSAGE( "ips::ReplacePattern1() test failed", l_ssResult == "onE two thrEE four fivE" );
  }

 void ReplaceTextInstance()
  {
	  unsigned int l_iNumReplacements = 0;
	  std::string l_ssResult = ips::ReplaceTextInstance( "one two three four five", "E", "Z", &l_iNumReplacements, true );

	  CPPUNIT_ASSERT_MESSAGE( "ips::ReplaceTextInstance() test failed", l_iNumReplacements == 4);

	  CPPUNIT_ASSERT_MESSAGE( "ips::ReplaceTextInstance() test failed", l_ssResult == "onZ two thrZZ four fivZ" );

	  l_ssResult = ips::ReplaceTextInstance( "one two three four five", "E", "Z", &l_iNumReplacements, false );

	  CPPUNIT_ASSERT_MESSAGE( "ips::ReplaceTextInstance() test failed", l_iNumReplacements == 0);

	  CPPUNIT_ASSERT_MESSAGE( "ips::ReplaceTextInstance() test failed", l_ssResult == "one two three four five" );
  }

  void ReplaceTextBetween()
  {
	  std::string l_ssResult = ips::ReplaceTextBetween( "begin two three end four five", "begin", "end", "replacement" );

	  CPPUNIT_ASSERT_MESSAGE( "ips::ReplaceTextBetween() test failed", l_ssResult == "replacementend four five" );
  }

  void AllNumeric()
  {
	  CPPUNIT_ASSERT_MESSAGE( "ips::AllNumeric() test failed", ips::AllNumeric("123456789") == true );
	  CPPUNIT_ASSERT_MESSAGE( "ips::AllNumeric() test failed", ips::AllNumeric("123456789abc") == false );
  }


  void AllAlphaNumeric()
  {
	  CPPUNIT_ASSERT_MESSAGE( "ips::AllAlphaNumeric() test failed", ips::AllAlphaNumeric("123456789") == true );
	  CPPUNIT_ASSERT_MESSAGE( "ips::AllAlphaNumeric() test failed", ips::AllAlphaNumeric("123456789abc") == true );
	  CPPUNIT_ASSERT_MESSAGE( "ips::AllAlphaNumeric() test failed", ips::AllAlphaNumeric("123456789abc[]") == false );
  }
};

} // End namespace stdextensions
} // end namespace ips

// Register the StdExtensionsTestFixture class with the test runner.
CPPUNIT_TEST_SUITE_REGISTRATION( ips::stdextensions::test );


#endif // STD_EXTENSIONS_TEST_FIXTURE_HEADER


