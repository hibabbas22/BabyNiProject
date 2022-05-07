# BabyNi

This project is the implementation of an application responsible for parsing, loading, aggregating, and visualizing data. 
Three APIs were created (Parser, Loader, Aggregator). 


To create the tables used in the database:
```sql
create table radio_link(   
      NetworkSId int,  
      DatatimeKey DateTime,  
      NeId int,  
      Object varchar(50),  
      Time Datetime,  
      Interval_t int,  
      Direction varchar(20),  
      NeAlias varchar(20),  
      NeType varchar(20),  
      RxLevelBelowTS1 int, 
      RxLevelBelowTS2 int, 
      MinRxLevel float, 
      MaxRxLevel float, 
      TxLevelAboveTS1 int, 
      MinTxLevel float, 
      MaxTxLevel float, 
      FailureDescription varchar(20), 
      TId varchar(20), 
      FarendTId varchar(20),
      Link varchar(20),
      Slot varchar(20), 
      Port varchar(20) 
);

```
Create hourly aggregation table:

```sql
create table HourlyAgg (   
       TimeKey datetime,   
       Link varchar(20),   
       Slot varchar(20),   
       Max_Rx_Level float,  
       Max_Tx_Level float,   
       Rsl_Deviation float 
       )
```
Create daily aggregation table:
```sql
create table DailyAgg (  
       TimeKey_daily datetime,   
       Link varchar(20),   
       Slot varchar(20),   
       Max_Rx_Level_daily float,  
       Max_Tx_Level_daily float , 
       Rsl_Deviation_daily float 
       )
```
