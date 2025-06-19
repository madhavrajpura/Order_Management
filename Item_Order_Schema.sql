----------------- Creating the tables for the Item_Order_DB ----------------------------

-- Role --

CREATE TABLE if not exists public."Role"
(
	"Id" serial not null primary key,
	"Name" VARCHAR(30) not null
);

-- User --

CREATE TABLE IF NOT EXISTS public."User"
(
	"Id" serial NOT NULL PRIMARY key,
	"Username" VARCHAR(50) not null UNIQUE,
	"Password" text not null,
	"Email" TEXT NOT NULL UNIQUE,
	"LogoutAt" TIMESTAMP,
	"RoleId" int Not null REFERENCES "Role"("Id"),
	"IsDelete" BOOLEAN not null  DEFAULT false,
	"CreatedAt" TIMESTAMP not null default now(),
	"UpdatedAt" Timestamp,
	"DeletedAt" Timestamp,
	"CreatedBy" int references "User"("Id"),
	"UpdatedBy" int references "User"("Id"),
	"DeletedBy" int references "User"("Id")
);

-- Items --

create TABLE if not exists public."Item"
(
	"Id" serial not null primary key,
	"Name" varchar(100) not null,
	"Price" DECIMAL(10,2) not null,
	"IsDelete" BOOLEAN not null  DEFAULT false,
	"CreatedAt" TIMESTAMP not null default now(),
	"UpdatedAt" Timestamp,
	"DeletedAt" Timestamp,
	"CreatedBy" int not null references "User"("Id"),
	"UpdatedBy" int references "User"("Id"),
	"DeletedBy" int references "User"("Id")
);

-- Order -- 

CREATE TABLE if not exists public."Order"
(
	"Id" serial not null primary key,
	"CustomerName" VARCHAR(100) not null,
	"UserId" int not null references "User"("Id"),
	"OrderAmount" DECIMAL(10,2) not null,
	"ItemId" int not null references "Item"("Id"),
	"ItemName" varchar(100) not null,
	"Quantity" int not null check("Quantity" > 0),
	"OrderDate" Timestamp not null default now(),
	"DeliveryDate" Timestamp not null,	
	"Price" DECIMAL(10,2) not NULL,
	"IsDelete" BOOLEAN not null  DEFAULT false,
	"CreatedAt" TIMESTAMP not null default now(),
	"UpdatedAt" Timestamp,
	"DeletedAt" Timestamp,
	"CreatedBy" int not null references "User"("Id"),
	"UpdatedBy" int references "User"("Id"),
	"DeletedBy" int references "User"("Id")
);

------------- Notification -------------
	
CREATE TABLE IF NOT EXISTS public."Notification" (
    "Id" SERIAL PRIMARY KEY,
    "Message" VARCHAR(500) NOT NULL,
    "CreatedAt" TIMESTAMP DEFAULT now(),
    "CreatedBy" INT REFERENCES public."User"("Id") NOT NULL,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE
);
 
------------- User Notification -------------

CREATE TABLE IF NOT EXISTS public."UserNotification" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INT REFERENCES public."User"("Id") NOT NULL,
    "NotificationId" INT REFERENCES public."Notification"("Id") NOT NULL,
    "IsRead" BOOLEAN NOT NULL DEFAULT FALSE,
    "ReadAt" TIMESTAMP	
);

--------------- Trigger Function when Item is added -------------------


CREATE OR REPLACE FUNCTION public.notify_new_item()
RETURNS TRIGGER AS $$
BEGIN
	INSERT INTO public."Notification" ("Message", "CreatedBy")
    VALUES ('New item added: ' || NEW."Name", NEW."CreatedBy");
    
	RETURN NEW;
    EXCEPTION WHEN OTHERS THEN
        RAISE WARNING 'Failed to create notification for new item: %', SQLERRM;
        RETURN NEW;
END;
$$ LANGUAGE plpgsql;

--------------- Trigger Function when Notification is added ------------------- 


CREATE OR REPLACE FUNCTION public.add_in_user_notification()
RETURNS TRIGGER AS $$
DECLARE
	user_notify_record RECORD;
BEGIN
	BEGIN

		-- With the Help of loop--
	
		-- FOR user_notify_record IN SELECT "Id" FROM public."User" WHERE "Id" != New."CreatedBy"
  --       LOOP
  --         	INSERT INTO public."UserNotification" ("UserId", "NotificationId")
  --          	VALUES (user_notify_record."Id", New."Id");
  --      	END LOOP;

	 	INSERT INTO public."UserNotification" ("UserId", "NotificationId")
    	SELECT u."Id", NEW."Id"
    	FROM public."User" u
    	WHERE u."Id" <> NEW."CreatedBy";
    	RETURN NEW;

		-- RETURN NEW;
		EXCEPTION WHEN OTHERS THEN
        	RAISE WARNING 'Failed to add entry in UserNotification';
        	RETURN NEW;
	END;
END;
$$ LANGUAGE plpgsql;

----------- CAll Trigger --------------

-- 1 --

CREATE TRIGGER trg_after_item_insert
AFTER INSERT ON public."Item"
FOR EACH ROW
EXECUTE FUNCTION public.notify_new_item();

-- 2 --

CREATE OR REPLACE TRIGGER trg_after_notification_insert
AFTER INSERT ON public."Notification"
FOR EACH ROW
EXECUTE FUNCTION public.add_in_user_notification();

---------- Combine Function -----------

CREATE OR REPLACE FUNCTION public.notify_new_item()
RETURNS TRIGGER AS $$
DECLARE
    notification_id INTEGER;
    user_notify_record RECORD;
BEGIN
    BEGIN
	
        INSERT INTO public."Notification" ("Message", "CreatedBy")
        VALUES ('New item added: ' || NEW."Name", NEW."CreatedBy")
        RETURNING "Id" INTO notification_id;
        
        FOR user_notify_record IN SELECT "Id" FROM public."User" WHERE "Id" != New."CreatedBy"
        LOOP
            INSERT INTO public."UserNotification" ("UserId", "NotificationId")
            VALUES (user_notify_record."Id", notification_id);
        END LOOP;
        
        RETURN NEW;
    EXCEPTION WHEN OTHERS THEN
        RAISE WARNING 'Failed to create notification for new item: %', SQLERRM;
        RETURN NEW;
    END;
END;
$$ LANGUAGE plpgsql;




