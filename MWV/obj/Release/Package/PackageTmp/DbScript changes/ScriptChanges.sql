/*
-----------------------------------------------------------------------------
DO NOT RUN THIS SCRIPT DIRECTLY ON SERVER, 
PLEASE VERIFY THE DATABASE ON WHICH YOU ARE RUNNING THE SCRIPT
ALSO VERIFY IF THESE CHANGES ARE ALREADY PRESENT ON SERVER

IDEALLY, RUN IT ONE BY ONE...
------------------------------------------------------------------------
*/


-- Added by Ramesh on 02 July
-- Need to store sidecut_id into the database

ALTER TABLE dbo.ProductionChilds ADD
                sidecut_id int NULL


--Added by Ramesh on 07 July
-- Need a remarks field in customer to capture remarks given by Planner 

ALTER TABLE dbo.Customers ADD
                remarks nvarchar(500) NULL
GO



-- Added by Rajni on 09 July 2015
-- Script creates Cores table and adds 2 rows to the DB

CREATE TABLE [dbo].[Cores](
	[core_code] [nvarchar](15) NOT NULL,
	[description] [nvarchar](100) NULL,
  CONSTRAINT [PK_Cores] PRIMARY KEY CLUSTERED (
	[core_code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[Cores]
            ([core_code]
            ,[description])
      VALUES
            (3,'3 inches')
INSERT INTO [dbo].[Cores]
            ([core_code]
            ,[description])
      VALUES
            (4,'4 inches')
GO



---------------------
-- Created by Rajni on 10 July 2015
-- BF table we need to change description from nchar to nvarchar

BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_Bfs
	(
	bf_code nvarchar(15) NOT NULL,
	description nvarchar(100) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Bfs SET (LOCK_ESCALATION = TABLE)
GO
IF EXISTS(SELECT * FROM dbo.Bfs)
	 EXEC('INSERT INTO dbo.Tmp_Bfs (bf_code, description)
		SELECT bf_code, CONVERT(nvarchar(100), description) FROM dbo.Bfs WITH (HOLDLOCK TABLOCKX)')
GO
ALTER TABLE dbo.Products
	DROP CONSTRAINT FK_Product_BF
GO
ALTER TABLE dbo.ProductionTimelines
	DROP CONSTRAINT FK_ProductionTimeline_Bfs
GO
DROP TABLE dbo.Bfs
GO
EXECUTE sp_rename N'dbo.Tmp_Bfs', N'Bfs', 'OBJECT' 
GO
ALTER TABLE dbo.Bfs ADD CONSTRAINT
	PK_BF PRIMARY KEY CLUSTERED 
	(
	bf_code
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.ProductionTimelines ADD CONSTRAINT
	FK_ProductionTimeline_Bfs FOREIGN KEY
	(
	bf_code
	) REFERENCES dbo.Bfs
	(
	bf_code
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.ProductionTimelines SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Products ADD CONSTRAINT
	FK_Product_BF FOREIGN KEY
	(
	bf_code
	) REFERENCES dbo.Bfs
	(
	bf_code
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Products SET (LOCK_ESCALATION = TABLE)
GO
COMMIT


-- Script to add deckle_approvals table
-- added by Ramesh on 15 July 15

BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Papermills SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Gsms SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Bfs SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Shades SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.deckle_approvals
	(
	dm_id int NOT NULL IDENTITY (1, 1),
	request_date datetime NULL,
	bf_code nvarchar(15) NULL,
	gsm_code nvarchar(15) NULL,
	shade_code nvarchar(15) NULL,
	papermill_id int NULL,
	matched_sizes nvarchar(500) NULL,
	required_size decimal(12, 4) NULL,
	required_weight decimal(12, 4) NULL,
	approver_aspnetuserid nvarchar(128) NULL,
	approved_on datetime NULL,
	action varchar(20) NULL,
	remarks nvarchar(500) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.deckle_approvals ADD CONSTRAINT
	PK_deckle_approvals PRIMARY KEY CLUSTERED 
	(
	dm_id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.deckle_approvals ADD CONSTRAINT
	FK_deckle_approvals_Bfs FOREIGN KEY
	(
	bf_code
	) REFERENCES dbo.Bfs
	(
	bf_code
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.deckle_approvals ADD CONSTRAINT
	FK_deckle_approvals_Gsms FOREIGN KEY
	(
	gsm_code
	) REFERENCES dbo.Gsms
	(
	gsm_code
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.deckle_approvals ADD CONSTRAINT
	FK_deckle_approvals_Shades FOREIGN KEY
	(
	shade_code
	) REFERENCES dbo.Shades
	(
	shade_code
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.deckle_approvals ADD CONSTRAINT
	FK_deckle_approvals_Papermills FOREIGN KEY
	(
	papermill_id
	) REFERENCES dbo.Papermills
	(
	papermill_id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.deckle_approvals SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

-- script to add Status field in Papermills and production_child & new table papermill_logs
-- added on 16 July 2015 by Ramesh

BEGIN TRANSACTION
GO
ALTER TABLE dbo.Papermills ADD
	status nvarchar(10) NULL
GO
COMMIT

BEGIN TRANSACTION
GO
ALTER TABLE dbo.ProductionChilds ADD
	status nvarchar(15) NULL
GO
COMMIT

-- add papermill_logs table

BEGIN TRANSACTION
GO
CREATE TABLE dbo.papermill_logs
	(
	pm_log_id int NOT NULL IDENTITY (1, 1),
	papermill_id int NOT NULL,
	stop_datetime datetime NULL,
	estimated_start datetime NULL,
	aspnetuser_id nvarchar(128) NULL,
	actual_start datetime NULL,
	remarks nvarchar(2000) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.papermill_logs ADD CONSTRAINT
	PK_papermill_logs PRIMARY KEY CLUSTERED 
	(
	pm_log_id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.papermill_logs ADD CONSTRAINT
	FK_papermill_logs_Papermills FOREIGN KEY
	(
	papermill_id
	) REFERENCES dbo.Papermills
	(
	papermill_id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.papermill_logs ADD CONSTRAINT
	FK_papermill_logs_AspNetUsers FOREIGN KEY
	(
	aspnetuser_id
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.papermill_logs SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

-- Added 3 fields in truck_dispatch tables to track who inwarded, outwarded and loaded the truck
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Truck_dispatches ADD
	inward_by nvarchar(128) NULL,
	outward_by nvarchar(128) NULL,
	loaded_by nvarchar(128) NULL
GO
COMMIT



-- added on 22 July
-- Change status in Orders
  update [dbo].[Orders] set status = 'Under Planning' where status = 'Open'
  update [dbo].[Order_products] set status = 'Under Planning' where status = 'Open'
  update [dbo].[Orders] set status = 'Planned' where status = 'Scheduled'
  update [dbo].[Order_products] set status = 'Planned' where status = 'Scheduled'
  update [dbo].[Orders] set status = 'In Warehouse' where status = 'Dispatch Ready'
  update [dbo].[Order_products] set status = 'In Warehouse' where status = 'Dispatch Ready'


 -- added by Ramesh 2 fields to track approvals used in Deckle

ALTER TABLE dbo.deckle_approvals ADD
	used int NULL,
	used_on datetime NULL
GO

-- added by manisha 1 fields to Orders table

ALTER TABLE Orders
ADD approved_by nvarchar(100)


-- added by Ramesh on 28 July for finance module
ALTER TABLE dbo.Customers ADD
	credit_limit decimal(18, 2) NULL,
	credit_days int NULL
GO

-- added by Rajni on 3 Aug 2015 for change request (#36 in Schedule sheet)
ALTER TABLE dbo.Order_products ADD
	requested_delivery_date datetime NULL

-- update requested delivery date of all existing orders
update op set op.requested_delivery_date = o.requested_delivery_date
from Orders o inner join order_products op on o.order_id = op.order_id

-- Changed field size of customerPO to 45 chars by Ramesh
-- run the full script as one batch
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.orders
	DROP CONSTRAINT FK_orders_Customer
GO
ALTER TABLE dbo.Customers SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.orders
	DROP CONSTRAINT FK_orders_Agent
GO
ALTER TABLE dbo.Agents SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_orders
	(
	order_id int NOT NULL IDENTITY (1, 1),
	order_date datetime NULL,
	agent_id int NULL,
	customer_id int NULL,
	status nvarchar(20) NULL,
	requested_delivery_date datetime NULL,
	remarks nvarchar(2000) NULL,
	amount decimal(18, 4) NULL,
	papermill_id int NULL,
	created_on datetime NULL,
	created_by nvarchar(100) NULL,
	modified_on datetime NULL,
	modified_by nvarchar(100) NULL,
	customerpo nvarchar(45) NULL,
	comments nvarchar(500) NULL,
	approved_by nvarchar(100) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_orders SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_orders ON
GO
IF EXISTS(SELECT * FROM dbo.orders)
	 EXEC('INSERT INTO dbo.Tmp_orders (order_id, order_date, agent_id, customer_id, status, requested_delivery_date, remarks, amount, papermill_id, created_on, created_by, modified_on, modified_by, customerpo, comments, approved_by)
		SELECT order_id, order_date, agent_id, customer_id, status, requested_delivery_date, remarks, amount, papermill_id, created_on, created_by, modified_on, modified_by, customerpo, comments, approved_by FROM dbo.orders WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_orders OFF
GO
ALTER TABLE dbo.order_products
	DROP CONSTRAINT FK_order_products_orders
GO
ALTER TABLE dbo.Truck_dispatch_details
	DROP CONSTRAINT FK_Truck_dispatch_details_Orders1
GO
DROP TABLE dbo.orders
GO
EXECUTE sp_rename N'dbo.Tmp_orders', N'orders', 'OBJECT' 
GO
ALTER TABLE dbo.orders ADD CONSTRAINT
	PK_orders PRIMARY KEY CLUSTERED 
	(
	order_id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.orders ADD CONSTRAINT
	FK_orders_Agent FOREIGN KEY
	(
	agent_id
	) REFERENCES dbo.Agents
	(
	agent_id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.orders ADD CONSTRAINT
	FK_orders_Customer FOREIGN KEY
	(
	customer_id
	) REFERENCES dbo.Customers
	(
	customer_id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Truck_dispatch_details ADD CONSTRAINT
	FK_Truck_dispatch_details_Orders1 FOREIGN KEY
	(
	order_id
	) REFERENCES dbo.orders
	(
	order_id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Truck_dispatch_details SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.order_products ADD CONSTRAINT
	FK_order_products_orders FOREIGN KEY
	(
	order_id
	) REFERENCES dbo.orders
	(
	order_id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.order_products SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
-- till here


--added 2 fields to aspnet users table by Ramesh on 11Aug

 ALTER TABLE dbo.AspNetUsers ADD
	city nvarchar(50) NULL,
	state nvarchar(50) NULL
GO

--added is_deckled field in orders table
ALTER TABLE dbo.orders ADD
	is_deckled bit NULL
GO

--added by Ramesh on 18Aug 
-- Alerts, emails and Messages table
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Email_templates
	(
	email_action_key nvarchar(50) NOT NULL,
	email_subject nvarchar(200) NULL,
	email_body nvarchar(4000) NULL,
	to_role nvarchar(50) NULL,
	cc_role nvarchar(50) NULL,
	bcc nvarchar(100) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Email_templates ADD CONSTRAINT
	PK_Email_templates PRIMARY KEY CLUSTERED 
	(
	email_action_key
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Email_templates SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Agents SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Messages
	(
	message_id int NOT NULL IDENTITY (1, 1),
	message_action nvarchar(50) NULL,
	message_for_role nvarchar(50) NULL,
	message_for_agentid int NULL,
	message_text nvarchar(4000) NULL,
	message_subject nvarchar(200) NULL,
	recipient1 nvarchar(100) NULL,
	recipient2 nvarchar(100) NULL,
	cc1 nvarchar(100) NULL,
	cc2 nvarchar(100) NULL,
	bcc1 nvarchar(100) NULL,
	bcc2 nvarchar(100) NULL,
	created_on datetime NULL,
	created_by nvarchar(100) NULL,
	attachment1 nvarchar(100) NULL,
	attachment2 nvarchar(100) NULL,
	status nvarchar(50) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Messages ADD CONSTRAINT
	PK_Messages PRIMARY KEY CLUSTERED 
	(
	message_id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Messages ADD CONSTRAINT
	FK_Messages_Agents FOREIGN KEY
	(
	message_for_agentid
	) REFERENCES dbo.Agents
	(
	agent_id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Messages SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Alerts
	(
	alert_id int NOT NULL IDENTITY (1, 1),
	alert_action nvarchar(50) NULL,
	alert_for_role nvarchar(50) NULL,
	alert_for_agentid int NULL,
	alert_text nvarchar(500) NULL,
	alert_subject nvarchar(100) NULL,
	created_on datetime NULL,
	created_by nvarchar(100) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Alerts ADD CONSTRAINT
	PK_Alerts PRIMARY KEY CLUSTERED 
	(
	alert_id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Alerts ADD CONSTRAINT
	FK_Alerts_Agents FOREIGN KEY
	(
	alert_for_agentid
	) REFERENCES dbo.Agents
	(
	agent_id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Alerts SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

-- Added by Ramesh on 24 Aug
-- Actualend in Production child and production jumbo table
BEGIN TRANSACTION
GO
ALTER TABLE dbo.ProductionJumboes ADD
	actual_end datetime NULL
GO
ALTER TABLE dbo.ProductionChilds ADD
	actual_end datetime NULL
GO
COMMIT

--added by RK on 25Aug
ALTER TABLE dbo.Bfs ADD
	created_on datetime NULL,
	created_by nvarchar(100) NULL,
	modified_on datetime NULL,
	modified_by nvarchar(100) NULL
GO

ALTER TABLE dbo.Gsms ADD
	created_on datetime NULL,
	created_by nvarchar(100) NULL,
	modified_on datetime NULL,
	modified_by nvarchar(100) NULL
GO

ALTER TABLE dbo.shades ADD
	created_on datetime NULL,
	created_by nvarchar(100) NULL,
	modified_on datetime NULL,
	modified_by nvarchar(100) NULL
GO

ALTER TABLE dbo.Products ADD
	created_on datetime NULL,
	created_by nvarchar(100) NULL,
	modified_on datetime NULL,
	modified_by nvarchar(100) NULL
GO

ALTER TABLE dbo.Product_prices ADD
	created_on datetime NULL,
	created_by nvarchar(100) NULL,
	modified_on datetime NULL,
	modified_by nvarchar(100) NULL
GO

ALTER TABLE dbo.Productiontimelines ADD
	created_on datetime NULL,
	created_by nvarchar(100) NULL,
	modified_on datetime NULL,
	modified_by nvarchar(100) NULL
GO

ALTER TABLE dbo.Cores ADD
	created_on datetime NULL,
	created_by nvarchar(100) NULL,
	modified_on datetime NULL,
	modified_by nvarchar(100) NULL
GO

-- added by RK on 27Aug to store PDF file name
BEGIN TRANSACTION
ALTER TABLE dbo.orders ADD
	pdf_file nvarchar(100) NULL
GO
ALTER TABLE dbo.ProductionRuns ADD
	pdf_file nvarchar(100) NULL
GO
COMMIT 


--Added by dhananjay , added  primery key in  [ProductionTimelines]

BEGIN TRANSACTION
GO
ALTER TABLE dbo.ProductionTimelines ADD
	production_timeline_id int NOT NULL IDENTITY (1, 1)
GO
ALTER TABLE dbo.ProductionTimelines ADD CONSTRAINT
	PK_ProductionTimelines PRIMARY KEY CLUSTERED 
	(
	production_timeline_id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.ProductionTimelines SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

--added new field in aspnetusers
BEGIN TRANSACTION

ALTER TABLE dbo.AspNetUsers ADD
	contactperson nvarchar(100) NULL
Commit

-- Missing Foreigh Keys added on 31 Aug
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Agents SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Customers ADD CONSTRAINT
	FK_Customers_Agents FOREIGN KEY
	(
	agent_id
	) REFERENCES dbo.Agents
	(
	agent_id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Customers SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

BEGIN TRANSACTION
GO
ALTER TABLE dbo.orders ADD CONSTRAINT
	FK_orders_Papermills FOREIGN KEY
	(
	papermill_id
	) REFERENCES dbo.Papermills
	(
	papermill_id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.orders SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

-- added on 04Sept by RK to store Start and end date times from Navision
ALTER TABLE dbo.ProductionChilds ADD
	external_startdate datetime NULL,
	external_enddate datetime NULL
GO

-- added on 04Sept by RK to store viewed status of the messages
BEGIN TRANSACTION
ALTER TABLE dbo.Messages ADD
	viewed int NULL
GO
ALTER TABLE dbo.Alerts ADD
	viewed int NULL
GO
COMMIT

-- added on 07Sept by RK to track Machinehead in Alerts and messages

BEGIN TRANSACTION
GO
ALTER TABLE dbo.Alerts ADD
	machinehead_aspnetuserid nvarchar(128) NULL
GO
ALTER TABLE dbo.Alerts ADD CONSTRAINT
	FK_Alerts_AspNetUsers FOREIGN KEY
	(
	machinehead_aspnetuserid
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE dbo.Messages ADD
	machinehead_aspnetuserid nvarchar(128) NULL
GO
ALTER TABLE dbo.Messages ADD CONSTRAINT
	FK_Messages_AspNetUsers FOREIGN KEY
	(
	machinehead_aspnetuserid
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
Commit

-- added by RK on 08 Sept - Marking for Deckle to be printed in Prod Plan
BEGIN TRANSACTION
GO
ALTER TABLE dbo.ProductionChilds ADD
	marking nvarchar(50) NULL
GO
COMMIT

--added by RK on 10 Sept 
--   - create schedule table
--   - Add column to order_products table to identify products assigned to Schedule
--   - Add column to ProductionRun table to identify productionPlans attached to schedule

BEGIN TRANSACTION
GO
CREATE TABLE dbo.Schedules
	(
	schedule_id int NOT NULL IDENTITY (1, 1),
	papermill_id int NULL,
	shade_code nvarchar(15) NULL,
	start_date datetime NULL,
	end_date datetime NULL,
	created_on datetime NULL,
	created_by nvarchar(100) NULL,
	modified_on datetime NULL,
	modified_by nvarchar(100) NULL,
	status nvarchar(15) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Schedules ADD CONSTRAINT
	PK_Schedules PRIMARY KEY CLUSTERED 
	(
	schedule_id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Schedules ADD CONSTRAINT
	FK_Schedules_Papermills FOREIGN KEY
	(
	papermill_id
	) REFERENCES dbo.Papermills
	(
	papermill_id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Schedules ADD CONSTRAINT
	FK_Schedules_Shades FOREIGN KEY
	(
	shade_code
	) REFERENCES dbo.Shades
	(
	shade_code
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT

BEGIN TRANSACTION
GO
ALTER TABLE dbo.order_products ADD
	schedule_id int NULL
GO
ALTER TABLE dbo.order_products ADD CONSTRAINT
	FK_order_products_Schedules FOREIGN KEY
	(
	schedule_id
	) REFERENCES dbo.Schedules
	(
	schedule_id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.order_products SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

BEGIN TRANSACTION
GO
ALTER TABLE dbo.ProductionRuns ADD
	schedule_id int NULL
GO
ALTER TABLE dbo.ProductionRuns ADD CONSTRAINT
	FK_ProductionRuns_Schedules FOREIGN KEY
	(
	schedule_id
	) REFERENCES dbo.Schedules
	(
	schedule_id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT

--added on 11 Sept 2015 -- new fields in customer table and aspnet table for agent's first and last name
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Customers ADD
	cust_code nvarchar(15) NULL,
	firstname nvarchar(15) NULL,
	lastname nvarchar(15) NULL
GO
ALTER TABLE dbo.AspNetUsers ADD
	firstname nvarchar(15) NULL,
	lastname nvarchar(15) NULL
GO
COMMIT

-- added on 11Sept 2015 -- new fields in order table to store ratings given by agent on order
BEGIN TRANSACTION
GO
ALTER TABLE dbo.orders ADD
	rating int NULL,
	rating_remarks nvarchar(1000) NULL,
	rated_on datetime NULL,
	rated_by nvarchar(100) NULL
GO
ALTER TABLE dbo.Messages ADD
	order_id int NULL
GO
ALTER TABLE dbo.Messages ADD CONSTRAINT
	FK_Messages_orders FOREIGN KEY
	(
	order_id
	) REFERENCES dbo.orders
	(
	order_id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

COMMIT


--added on 12Sept 2015 - for customer login and ordering
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Customers ADD
	aspnetuser_id nvarchar(128) NULL,
	login_enabled int NULL,
	order_enabled int NULL
GO
ALTER TABLE dbo.Customers ADD CONSTRAINT
	FK_Customers_AspNetUsers FOREIGN KEY
	(
	aspnetuser_id
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT

-- added on 12Sept - Changed Field length of Message_text on Messages table to MAX
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Messages
	DROP CONSTRAINT FK_Messages_AspNetUsers
GO
ALTER TABLE dbo.AspNetUsers SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Messages
	DROP CONSTRAINT FK_Messages_Agents
GO
ALTER TABLE dbo.Agents SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Messages
	DROP CONSTRAINT FK_Messages_orders
GO
ALTER TABLE dbo.orders SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_Messages
	(
	message_id int NOT NULL IDENTITY (1, 1),
	message_action nvarchar(50) NULL,
	message_for_role nvarchar(50) NULL,
	message_for_agentid int NULL,
	message_text nvarchar(MAX) NULL,
	message_subject nvarchar(200) NULL,
	recipient1 nvarchar(100) NULL,
	recipient2 nvarchar(100) NULL,
	cc1 nvarchar(100) NULL,
	cc2 nvarchar(100) NULL,
	bcc1 nvarchar(100) NULL,
	bcc2 nvarchar(100) NULL,
	created_on datetime NULL,
	created_by nvarchar(100) NULL,
	attachment1 nvarchar(100) NULL,
	attachment2 nvarchar(100) NULL,
	status nvarchar(50) NULL,
	viewed int NULL,
	machinehead_aspnetuserid nvarchar(128) NULL,
	order_id int NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Messages SET (LOCK_ESCALATION = TABLE)
GO
SET IDENTITY_INSERT dbo.Tmp_Messages ON
GO
IF EXISTS(SELECT * FROM dbo.Messages)
	 EXEC('INSERT INTO dbo.Tmp_Messages (message_id, message_action, message_for_role, message_for_agentid, message_text, message_subject, recipient1, recipient2, cc1, cc2, bcc1, bcc2, created_on, created_by, attachment1, attachment2, status, viewed, machinehead_aspnetuserid, order_id)
		SELECT message_id, message_action, message_for_role, message_for_agentid, CONVERT(nvarchar(MAX), message_text), message_subject, recipient1, recipient2, cc1, cc2, bcc1, bcc2, created_on, created_by, attachment1, attachment2, status, viewed, machinehead_aspnetuserid, order_id FROM dbo.Messages WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Messages OFF
GO
DROP TABLE dbo.Messages
GO
EXECUTE sp_rename N'dbo.Tmp_Messages', N'Messages', 'OBJECT' 
GO
ALTER TABLE dbo.Messages ADD CONSTRAINT
	PK_Messages PRIMARY KEY CLUSTERED 
	(
	message_id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Messages ADD CONSTRAINT
	FK_Messages_orders FOREIGN KEY
	(
	order_id
	) REFERENCES dbo.orders
	(
	order_id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Messages ADD CONSTRAINT
	FK_Messages_Agents FOREIGN KEY
	(
	message_for_agentid
	) REFERENCES dbo.Agents
	(
	agent_id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Messages ADD CONSTRAINT
	FK_Messages_AspNetUsers FOREIGN KEY
	(
	machinehead_aspnetuserid
	) REFERENCES dbo.AspNetUsers
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT


-- added on 13Sept 15, Stock table to hold opening and daily stocks 
BEGIN TRANSACTION
GO
CREATE TABLE dbo.stocks
	(
	stock_id int NOT NULL IDENTITY (1, 1),
	stock_date datetime NULL,
	papermill_id int NULL,
	agent_id int NULL,
	customer_id int NULL,
	product_code nvarchar(15) NULL,
	shade_code nvarchar(15) NULL,
	opening_stock decimal(12, 4) NULL,
	stock_produced decimal(12, 4) NULL,
	stock_shipped decimal(12, 4) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.stocks ADD CONSTRAINT
	PK_stock PRIMARY KEY CLUSTERED 
	(
	stock_id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.stocks ADD CONSTRAINT
	FK_stock_Papermills FOREIGN KEY
	(
	papermill_id
	) REFERENCES dbo.Papermills
	(
	papermill_id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.stocks ADD CONSTRAINT
	FK_stock_Agents FOREIGN KEY
	(
	agent_id
	) REFERENCES dbo.Agents
	(
	agent_id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.stocks ADD CONSTRAINT
	FK_stock_Customers FOREIGN KEY
	(
	customer_id
	) REFERENCES dbo.Customers
	(
	customer_id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.stocks ADD CONSTRAINT
	FK_stock_Products FOREIGN KEY
	(
	product_code
	) REFERENCES dbo.Products
	(
	product_code
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.stocks ADD CONSTRAINT
	FK_stock_Shades FOREIGN KEY
	(
	shade_code
	) REFERENCES dbo.Shades
	(
	shade_code
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
--add customer in roles table
 insert into AspNetRoles(Id,Name) values('78a0f628-6367-42f6-b7d0-890277c2ac01','Customer');

--added on 19Sept to store reports
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Reports
	(
	report_id int NOT NULL IDENTITY (1, 1),
	created_by nvarchar(100) NULL,
	created_on datetime NULL,
	report_name nvarchar(500) NULL,
	report_criteria nvarchar(2000) NULL,
	file_name nvarchar(200) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Reports ADD CONSTRAINT
	PK_Reports PRIMARY KEY CLUSTERED 
	(
	report_id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Reports SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

