CREATE TABLE Region 
( 
  RegionID  NUMBER(3) NOT NULL, 
  RegionDescription  CHAR(50) NOT NULL, 
CONSTRAINT PK_Region 
  PRIMARY KEY (RegionID)
) 
/ 

CREATE TABLE Territories 
( 
  TerritoryID  VARCHAR2(20) NOT NULL, 
  TerritoryDescription  CHAR(50) NOT NULL, 
  RegionID  NUMBER(3) NOT NULL, 
CONSTRAINT PK_Territories 
  PRIMARY KEY (TerritoryID), 
CONSTRAINT FK_Territories_Region FOREIGN KEY (RegionID) REFERENCES Region(RegionID)
) 
/ 

CREATE TABLE Categories 
( 
  CategoryID  NUMBER(3,0) NOT NULL, 
  CategoryName  VARCHAR2(15) NOT NULL, 
  Description  VARCHAR2(300), 
  Picture  LONG RAW, 
CONSTRAINT PK_Categories 
  PRIMARY KEY (CategoryID)
) 
/ 

CREATE TABLE Suppliers 
( 
  SupplierID  NUMBER(5,0) NOT NULL, 
  CompanyName  VARCHAR2(40) NOT NULL, 
  ContactName  VARCHAR2(30), 
  ContactTitle  VARCHAR2(30), 
  Address  VARCHAR2(60), 
  City  VARCHAR2(15), 
  Region  VARCHAR2(15), 
  PostalCode  VARCHAR2(10), 
  Country  VARCHAR2(15), 
  Phone  VARCHAR2(24), 
  Fax  VARCHAR2(24), 
  HomePage  VARCHAR2(200), 
CONSTRAINT PK_Suppliers 
  PRIMARY KEY (SupplierID)
) 
/ 

CREATE TABLE Products 
( 
  ProductID  NUMBER(5,0) NOT NULL, 
  ProductName  VARCHAR2(40) NOT NULL, 
  SupplierID  NUMBER(5,0), 
  CategoryID  NUMBER(3,0), 
  QuantityPerUnit  VARCHAR2(20), 
  UnitPrice  NUMBER, 
  UnitsInStock  NUMBER(10,0), 
  UnitsOnOrder  NUMBER(10,0), 
  ReorderLevel  NUMBER(10,0), 
  Discontinued  NUMBER(1) NOT NULL, 
CONSTRAINT PK_Products 
  PRIMARY KEY (ProductID), 
CONSTRAINT CK_Products_UnitPrice   CHECK ((UnitPrice >= 0)), 
CONSTRAINT CK_ReorderLevel   CHECK ((ReorderLevel >= 0)), 
CONSTRAINT CK_UnitsInStock   CHECK ((UnitsInStock >= 0)), 
CONSTRAINT CK_UnitsOnOrder   CHECK ((UnitsOnOrder >= 0)), 
CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID), 
CONSTRAINT FK_Products_Suppliers FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID)
) 
/ 

CREATE TABLE Shippers 
( 
  ShipperID  NUMBER(3) NOT NULL, 
  CompanyName  VARCHAR2(40) NOT NULL, 
  Phone  VARCHAR2(24), 
CONSTRAINT PK_Shippers 
  PRIMARY KEY (ShipperID)
) 
/ 

CREATE TABLE Customers 
( 
  CustomerID  CHAR(5) NOT NULL, 
  CompanyName  VARCHAR2(40) NOT NULL, 
  ContactName  VARCHAR2(30), 
  ContactTitle  VARCHAR2(30), 
  Address  VARCHAR2(60), 
  City  VARCHAR2(15), 
  Region  VARCHAR2(15), 
  PostalCode  VARCHAR2(10), 
  Country  VARCHAR2(15), 
  Phone  VARCHAR2(24), 
  Fax  VARCHAR2(24), 
CONSTRAINT PK_Customers 
  PRIMARY KEY (CustomerID)
) 
/ 

CREATE TABLE Employees 
( 
  EmployeeID  NUMBER(10) NOT NULL, 
  LastName  VARCHAR2(20) NOT NULL, 
  FirstName  VARCHAR2(10) NOT NULL, 
  Title  VARCHAR2(30), 
  TitleOfCourtesy  VARCHAR2(25), 
  BirthDate  DATE, 
  HireDate  DATE, 
  Address  VARCHAR2(60), 
  City  VARCHAR2(15), 
  Region  VARCHAR2(15), 
  PostalCode  VARCHAR2(10), 
  Country  VARCHAR2(15), 
  HomePhone  VARCHAR2(24), 
  Extension  VARCHAR2(4), 
  Photo  LONG RAW, 
  Notes  VARCHAR2(600), 
  ReportsTo  NUMBER(10), 
  PhotoPath  VARCHAR2(255), 
CONSTRAINT PK_Employees 
  PRIMARY KEY (EmployeeID), 
CONSTRAINT FK_Employees_Employees FOREIGN KEY (ReportsTo) REFERENCES Employees(EmployeeID)
) 
/ 

CREATE TABLE EmployeeTerritories 
( 
  EmployeeID  NUMBER(10) NOT NULL, 
  TerritoryID  VARCHAR2(20) NOT NULL, 
CONSTRAINT PK_EmpTerritories 
  PRIMARY KEY (EmployeeID, TerritoryID), 
CONSTRAINT FK_EmpTerri_Employees FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID), 
CONSTRAINT FK_EmpTerri_Territories FOREIGN KEY (TerritoryID) REFERENCES Territories(TerritoryID)
) 
/ 

CREATE TABLE CustomerDemographics 
( 
  CustomerTypeID  CHAR(10) NOT NULL, 
  CustomerDesc  LONG, 
CONSTRAINT PK_CustomerDemographics 
  PRIMARY KEY (CustomerTypeID)
) 
/ 

CREATE TABLE CustomerCustomerDemo 
( 
  CustomerID  CHAR(5) NOT NULL, 
  CustomerTypeID  CHAR(10) NOT NULL, 
CONSTRAINT PK_CustomerDemo 
  PRIMARY KEY (CustomerID, CustomerTypeID), 
CONSTRAINT FK_CustomerDemo FOREIGN KEY (CustomerTypeID) REFERENCES CustomerDemographics(CustomerTypeID), 
CONSTRAINT FK_CustomerDemo_Customers FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID)
) 
/ 

CREATE TABLE Orders 
( 
  OrderID  NUMBER(19) NOT NULL, 
  CustomerID  CHAR(5), 
  EmployeeID  NUMBER(10), 
  TerritoryID  VARCHAR2(20), 
  OrderDate  DATE, 
  RequiredDate  DATE, 
  ShippedDate  DATE, 
  ShipVia  NUMBER(3), 
  Freight  NUMBER, 
  ShipName  VARCHAR2(40), 
  ShipAddress  VARCHAR2(60), 
  ShipCity  VARCHAR2(15), 
  ShipRegion  VARCHAR2(15), 
  ShipPostalCode  VARCHAR2(10), 
  ShipCountry  VARCHAR2(15), 
CONSTRAINT PK_Orders 
  PRIMARY KEY (OrderID), 
CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID), 
CONSTRAINT FK_Orders_Employees FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID), 
CONSTRAINT FK_Orders_Shippers FOREIGN KEY (ShipVia) REFERENCES Shippers(ShipperID),
CONSTRAINT FK_Orders_Territories FOREIGN KEY (TerritoryID) REFERENCES Territories(TerritoryID)
) 
/ 

CREATE TABLE OrderDetails 
( 
  OrderID  NUMBER(19) NOT NULL, 
  ProductID  NUMBER(5) NOT NULL, 
  UnitPrice  NUMBER NOT NULL, 
  Quantity  NUMBER(10) NOT NULL, 
  Discount  NUMBER NOT NULL, 
CONSTRAINT PK_Order_Details 
  PRIMARY KEY (OrderID, ProductID), 
CONSTRAINT CK_Discount   CHECK ((Discount >= 0 and Discount <= 1)), 
CONSTRAINT CK_Quantity   CHECK ((Quantity > 0)), 
CONSTRAINT CK_UnitPrice   CHECK ((UnitPrice >= 0)), 
CONSTRAINT FK_OrderDetails_Orders FOREIGN KEY (OrderID) REFERENCES Orders(OrderID), 
CONSTRAINT FK_OrderDetails_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
)
/

CREATE OR REPLACE VIEW TerritorySales as
SELECT 
    o.TerritoryId, sum(ot.Total) as Total
FROM 
    Orders o
INNER JOIN 
    (select od.OrderId, sum(od.UnitPrice * od.Quantity) as Total from OrderDetails od group by od.OrderId) ot
ON 
    o.OrderId = ot.OrderId
GROUP BY
    o.TerritoryId;
/