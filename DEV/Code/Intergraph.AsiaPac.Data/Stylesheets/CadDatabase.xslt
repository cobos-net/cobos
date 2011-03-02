<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" exclude-result-prefixes="msxsl" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://www.w3.org/2001/XMLSchema" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
   <xsl:output method="xml" indent="yes" encoding="utf-8" />
   <xsl:variable name="databaseTypes">
      <xsd:simpleType name="char" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="1" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_1" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="1" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_2" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="2" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_3" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="3" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_4" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="4" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_5" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="5" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_6" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="6" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_7" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="7" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_8" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="8" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_9" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="9" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_10" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="10" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_11" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="11" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_12" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="12" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_14" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="14" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_15" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="15" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_16" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="16" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_20" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="20" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_25" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="25" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_30" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="30" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_40" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="40" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_45" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="45" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_50" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="50" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_60" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="60" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_80" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="80" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_100" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="100" />
         </xsd:restriction>
      </xsd:simpleType>
      <xsd:simpleType name="string_240" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:restriction base="xsd:string">
            <xsd:maxLength value="240" />
         </xsd:restriction>
      </xsd:simpleType>
   </xsl:variable>
   <xsl:variable name="databaseTypesNodeSet" select="msxsl:node-set( $databaseTypes )" />
   <xsl:variable name="databaseTables">
      <xsd:element name="AEVEN" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:complexType>
            <xsd:sequence>
               <xsd:element name="AD_SEC" type="xsd:integer" minOccurs="1" />
               <xsd:element name="AD_TS" type="string_16" minOccurs="1" />
               <xsd:element name="AG_ID" type="string_9" minOccurs="1" />
               <xsd:element name="ALARM_LEV" type="xsd:integer" minOccurs="1" />
               <xsd:element name="AR_SEC" type="xsd:integer" minOccurs="0" />
               <xsd:element name="AR_TS" type="string_16" minOccurs="0" />
               <xsd:element name="ASSIGNED_UNITS" type="xsd:integer" minOccurs="1" />
               <xsd:element name="CDTS" type="string_16" minOccurs="1" />
               <xsd:element name="CPERS" type="xsd:integer" minOccurs="1" />
               <xsd:element name="CREATE_TERM" type="string_15" minOccurs="1" />
               <xsd:element name="CSEC" type="xsd:integer" minOccurs="1" />
               <xsd:element name="CTERM" type="string_15" minOccurs="1" />
               <xsd:element name="CURENT" type="string_1" minOccurs="1" />
               <xsd:element name="DEST_EID" type="xsd:integer" minOccurs="0" />
               <xsd:element name="DGROUP" type="string_5" minOccurs="1" />
               <xsd:element name="DISPASS_UNIT" type="string_10" minOccurs="0" />
               <xsd:element name="DS_SEC" type="xsd:integer" minOccurs="0" />
               <xsd:element name="DS_TS" type="string_16" minOccurs="0" />
               <xsd:element name="EID" type="xsd:integer" minOccurs="1" />
               <xsd:element name="ESZ" type="xsd:integer" minOccurs="0" />
               <xsd:element name="ETA" type="xsd:integer" minOccurs="0" />
               <xsd:element name="EVENT_STATUS" type="string_1" minOccurs="0" />
               <xsd:element name="EVT_REV_NUM" type="xsd:integer" minOccurs="1" />
               <xsd:element name="EX_EVT" type="string_1" minOccurs="0" />
               <xsd:element name="HOLD_DTS" type="string_16" minOccurs="0" />
               <xsd:element name="HOLD_TYPE" type="xsd:integer" minOccurs="0" />
               <xsd:element name="HOLD_UNT" type="string_10" minOccurs="0" />
               <xsd:element name="LATE_RUN" type="string_1" minOccurs="0" />
               <xsd:element name="LEV2" type="string_6" minOccurs="0" />
               <xsd:element name="LEV3" type="string_7" minOccurs="0" />
               <xsd:element name="LEV4" type="string_5" minOccurs="0" />
               <xsd:element name="LEV5" type="string_5" minOccurs="0" />
               <xsd:element name="LOI_FLAG_OLD" type="string_1" minOccurs="0" />
               <xsd:element name="MAJEVT_EVTY" type="string_10" minOccurs="0" />
               <xsd:element name="MAJEVT_LOC" type="string_10" minOccurs="0" />
               <xsd:element name="MUN" type="string_40" minOccurs="0" />
               <xsd:element name="NUM_1" type="string_12" minOccurs="1" />
               <xsd:element name="NUM_2" type="string_12" minOccurs="0" />
               <xsd:element name="NUM_3" type="string_12" minOccurs="0" />
               <xsd:element name="NUM_4" type="string_12" minOccurs="0" />
               <xsd:element name="OPEN_AND_CURENT" type="string_1" minOccurs="1" />
               <xsd:element name="PEND_DTS" type="string_16" minOccurs="0" />
               <xsd:element name="PRIM_MEMBER" type="xsd:integer" minOccurs="0" />
               <xsd:element name="PRIM_UNIT" type="string_10" minOccurs="0" />
               <xsd:element name="PRIORITY" type="string_1" minOccurs="1" />
               <xsd:element name="PROBE_FLAG" type="string_16" minOccurs="0" />
               <xsd:element name="QUAL1_OLD" type="string_8" minOccurs="0" />
               <xsd:element name="QUAL2_OLD" type="string_3" minOccurs="0" />
               <xsd:element name="QUAL3_OLD" type="string_3" minOccurs="0" />
               <xsd:element name="RESP_DOWN" type="string_1" minOccurs="0" />
               <xsd:element name="RESP_TIME" type="xsd:float" minOccurs="0" />
               <xsd:element name="REV_NUM" type="xsd:integer" minOccurs="1" />
               <xsd:element name="SCDTS" type="string_16" minOccurs="1" />
               <xsd:element name="SDTS" type="string_16" minOccurs="0" />
               <xsd:element name="SITFND" type="string_16" minOccurs="0" />
               <xsd:element name="SSEC" type="xsd:integer" minOccurs="0" />
               <xsd:element name="SUB_SITFND" type="string_16" minOccurs="0" />
               <xsd:element name="SUPP_INFO" type="xsd:integer" minOccurs="0" />
               <xsd:element name="TA_SEC" type="xsd:integer" minOccurs="0" />
               <xsd:element name="TA_TS" type="string_16" minOccurs="0" />
               <xsd:element name="TR_SEC" type="xsd:integer" minOccurs="0" />
               <xsd:element name="TR_TS" type="string_16" minOccurs="0" />
               <xsd:element name="UDTS" type="string_16" minOccurs="0" />
               <xsd:element name="UPERS" type="xsd:integer" minOccurs="0" />
               <xsd:element name="UTERM" type="string_15" minOccurs="0" />
               <xsd:element name="XCMT" type="string_80" minOccurs="0" />
               <xsd:element name="XDISPO_OLD" type="string_16" minOccurs="0" />
               <xsd:element name="XDOW" type="xsd:integer" minOccurs="0" />
               <xsd:element name="XDTS" type="string_16" minOccurs="0" />
               <xsd:element name="XPERS" type="xsd:integer" minOccurs="0" />
               <xsd:element name="XSEC" type="xsd:integer" minOccurs="0" />
               <xsd:element name="XTERM" type="string_15" minOccurs="0" />
               <xsd:element name="MFB_EVENT_STATUS" type="string_2" minOccurs="0" />
               <xsd:element name="MFB_FALSE_ALARM" type="string_3" minOccurs="0" />
               <xsd:element name="MFB_SIGNIFICANT" type="char" minOccurs="0" />
               <xsd:element name="MFB_SUSPICIOUS" type="char" minOccurs="0" />
               <xsd:element name="FSV_SHORT_TYCOD" type="string_10" minOccurs="0" />
               <xsd:element name="SENDID" type="xsd:integer" minOccurs="0" />
               <xsd:element name="MAS_RESP" type="string_10" minOccurs="0" />
               <xsd:element name="MAS_AGE" type="string_30" minOccurs="0" />
               <xsd:element name="MAS_GENDER" type="string_7" minOccurs="0" />
               <xsd:element name="MAS_CON" type="string_10" minOccurs="0" />
               <xsd:element name="PROQA_NUM" type="string_12" minOccurs="0" />
               <xsd:element name="VP_COHERENT" type="char" minOccurs="0" />
               <xsd:element name="VP_EVNT_PH" type="string_25" minOccurs="0" />
               <xsd:element name="VP_MEMBER_NO" type="string_20" minOccurs="0" />
               <xsd:element name="VP_SEE_COMP" type="char" minOccurs="0" />
               <xsd:element name="VP_SPK_ENG" type="char" minOccurs="0" />
               <xsd:element name="MAS_APPT_DTS" type="string_16" minOccurs="0" />
               <xsd:element name="MAS_PSYCH_CAT" type="string_1" minOccurs="0" />
               <xsd:element name="MAS_PSYCH_URGENCY" type="string_1" minOccurs="0" />
               <xsd:element name="PROQA_RESPONDER" type="string_240" minOccurs="0" />
               <xsd:element name="CREATE_BTN_TS" type="string_16" minOccurs="0" />
               <xsd:element name="CREATE_PERS" type="xsd:integer" minOccurs="0" />
               <xsd:element name="APPT_END_TS" type="string_16" minOccurs="0" />
               <xsd:element name="EN_SEC" type="xsd:integer" minOccurs="0" />
               <xsd:element name="EN_TS" type="string_16" minOccurs="0" />
               <xsd:element name="LOI_AVAIL_DTS" type="string_16" minOccurs="1" />
               <xsd:element name="REOPEN" type="string_1" minOccurs="1" />
               <xsd:element name="SUB_ENG" type="string_80" minOccurs="0" />
               <xsd:element name="SUB_TYCOD" type="string_16" minOccurs="0" />
               <xsd:element name="TYCOD" type="string_16" minOccurs="0" />
               <xsd:element name="TYP_ENG" type="string_80" minOccurs="0" />
               <xsd:element name="VDTS" type="string_16" minOccurs="0" />
               <xsd:element name="VSEC" type="xsd:integer" minOccurs="0" />
               <xsd:element name="RECOM_INCOMPLETE" type="string_1" minOccurs="1" />
               <xsd:element name="MAS_SCHED_DTS" type="string_16" minOccurs="0" />
               <xsd:element name="TERM_TYPE" type="string_25" minOccurs="0" />
               <xsd:element name="MAS_ACUITY" type="string_10" minOccurs="0" />
            </xsd:sequence>
         </xsd:complexType>
      </xsd:element>
      <xsd:element name="EVENT" xmlns="http://schemas.intergraph.com/asiapac/cad/datamodel" xmlns:cad="http://schemas.intergraph.com/asiapac/cad/datamodel">
         <xsd:complexType>
            <xsd:sequence>
               <xsd:element name="ANI_NUM" type="xsd:integer" minOccurs="0" />
               <xsd:element name="CALL_SOUR" type="string_8" minOccurs="0" />
               <xsd:element name="CCITY" type="string_25" minOccurs="0" />
               <xsd:element name="CDTS" type="string_16" minOccurs="1" />
               <xsd:element name="CLNAME" type="string_50" minOccurs="0" />
               <xsd:element name="CLRNUM" type="string_20" minOccurs="0" />
               <xsd:element name="CPERS" type="xsd:integer" minOccurs="1" />
               <xsd:element name="CSEC" type="xsd:integer" minOccurs="1" />
               <xsd:element name="CSTR_ADD" type="string_80" minOccurs="0" />
               <xsd:element name="CTERM" type="string_15" minOccurs="1" />
               <xsd:element name="CURENT" type="string_1" minOccurs="1" />
               <xsd:element name="DISPATCHED" type="string_1" minOccurs="0" />
               <xsd:element name="DOW" type="string_1" minOccurs="1" />
               <xsd:element name="EAPT" type="string_14" minOccurs="0" />
               <xsd:element name="EAREA" type="string_40" minOccurs="0" />
               <xsd:element name="ECOMPL" type="string_80" minOccurs="0" />
               <xsd:element name="EDIRPRE" type="string_4" minOccurs="0" />
               <xsd:element name="EDIRSUF" type="string_4" minOccurs="0" />
               <xsd:element name="EFEANME" type="string_240" minOccurs="0" />
               <xsd:element name="EFEATYP" type="string_4" minOccurs="0" />
               <xsd:element name="EID" type="xsd:integer" minOccurs="1" />
               <xsd:element name="EMUN" type="string_40" minOccurs="0" />
               <xsd:element name="ESTNUM" type="string_11" minOccurs="0" />
               <xsd:element name="FEA_MSLINK" type="xsd:integer" minOccurs="0" />
               <xsd:element name="HASH" type="xsd:integer" minOccurs="0" />
               <xsd:element name="LOC_COM" type="string_100" minOccurs="0" />
               <xsd:element name="LOI_DATA_OLD" type="string_1" minOccurs="0" />
               <xsd:element name="LOI_SEARCH" type="string_1" minOccurs="1" />
               <xsd:element name="NODEID" type="xsd:integer" minOccurs="0" />
               <xsd:element name="PAGE_ID" type="string_12" minOccurs="0" />
               <xsd:element name="PATIENT" type="string_45" minOccurs="0" />
               <xsd:element name="REV_NUM" type="xsd:integer" minOccurs="1" />
               <xsd:element name="SUB_ENG_OLD" type="string_80" minOccurs="0" />
               <xsd:element name="SUB_TYCOD_OLD" type="string_16" minOccurs="0" />
               <xsd:element name="TYCOD_OLD" type="string_16" minOccurs="0" />
               <xsd:element name="TYP_ENG_OLD" type="string_80" minOccurs="0" />
               <xsd:element name="UDTS" type="string_16" minOccurs="0" />
               <xsd:element name="UPDT_FLAG" type="string_15" minOccurs="1" />
               <xsd:element name="UPERS" type="xsd:integer" minOccurs="0" />
               <xsd:element name="UTERM" type="string_15" minOccurs="0" />
               <xsd:element name="X_CORD" type="xsd:integer" minOccurs="0" />
               <xsd:element name="XSTREET1" type="string_60" minOccurs="0" />
               <xsd:element name="XSTREET2" type="string_60" minOccurs="0" />
               <xsd:element name="Y_CORD" type="xsd:integer" minOccurs="0" />
               <xsd:element name="ALARM_ID" type="string_80" minOccurs="0" />
               <xsd:element name="CLI_JOB_NUM" type="string_7" minOccurs="0" />
               <xsd:element name="CLI_CABINET_NUM" type="string_12" minOccurs="0" />
               <xsd:element name="VER_MELWAY" type="string_11" minOccurs="0" />
               <xsd:element name="VER_ESMAP" type="string_11" minOccurs="0" />
               <xsd:element name="VER_REGION7" type="string_11" minOccurs="0" />
               <xsd:element name="VER_MELWAY_DEST" type="string_11" minOccurs="0" />
               <xsd:element name="VER_REGION15" type="string_11" minOccurs="0" />
               <xsd:element name="VER_REGION22" type="string_11" minOccurs="0" />
               <xsd:element name="VER_VICROADS" type="string_11" minOccurs="0" />
               <xsd:element name="BES_COMM_DIFFICULTY" type="string_20" minOccurs="0" />
               <xsd:element name="VER_CFA" type="string_11" minOccurs="0" />
               <xsd:element name="VER_OMMB" type="string_11" minOccurs="0" />
               <xsd:element name="ELOC_FLD1" type="string_5" minOccurs="0" />
               <xsd:element name="ELOC_FLD2" type="string_5" minOccurs="0" />
               <xsd:element name="ELOC_FLD3" type="string_5" minOccurs="0" />
               <xsd:element name="LOI_EVENT" type="string_1" minOccurs="1" />
               <xsd:element name="LOI_INF" type="string_1" minOccurs="1" />
               <xsd:element name="LOI_SPECSIT" type="string_1" minOccurs="1" />
               <xsd:element name="LOC_VER" type="string_1" minOccurs="1" />
               <xsd:element name="VERIFY_COMPLETE_DTS" type="string_16" minOccurs="0" />
               <xsd:element name="D_ECOMPL" type="string_80" minOccurs="0" />
            </xsd:sequence>
         </xsd:complexType>
      </xsd:element>
   </xsl:variable>
   <xsl:variable name="databaseTablesNodeSet" select="msxsl:node-set( $databaseTables )" />
</xsl:stylesheet>