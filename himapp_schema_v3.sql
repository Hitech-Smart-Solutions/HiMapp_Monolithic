-- ============================================================
-- HIMAPP 2.0 — Site Execution Schema (PostgreSQL) — v3
-- Naming: PascalCase tables, Header/Details pattern
--
-- PREREQUISITE: "Company", "Users", "Project", "Contractor"
-- must already exist (v1 script) before running this file.
--
-- Every table carries the standard audit columns:
--   "ID"                INT identity (internal PK)
--   "UniqueID"          UUID (external/public reference)
--   "IsActive"          soft-delete flag
--   "CreatedBy"         user who created the row
--   "CreatedDate"       creation timestamp
--   "LastModifiedBy"    user who last edited the row
--   "LastModifiedDate"  auto-updated on every UPDATE (trigger)
--
-- CHANGES IN v3 (from review):
--   1. New "UOM" master table; all "UOMID" columns now have FKs.
--   2. All business unique constraints converted to PARTIAL
--      unique indexes (WHERE "IsActive") so soft-deleted rows
--      no longer block re-creation / re-entry.
--   3. "DailyProgressDetails": added "AreaID" (area-wise DPR,
--      matches Planning/Manpower) + unique constraint to
--      prevent duplicate rows per header.
--   4. "DailyProgress"."TotalAmount" now kept in sync by a
--      trigger on "DailyProgressDetails".
--   5. Approval metadata ("ApprovedBy", "ApprovedDate",
--      "RejectionReason") added to all 4 status headers, plus
--      a trigger that blocks edits to APPROVED headers
--      (status must be moved away from APPROVED first).
--   6. "MatCount" renamed to "OtherCount" (consistent with
--      "ManpowerDetails"); redundant "Enabled" column dropped
--      from "ProjectActivity" ("IsActive" is the toggle).
--   7. "DailyLabor" now carries "Shift" (unique per
--      project/date/shift, same as "Manpower").
--   8. "ProjectID" and user-audit FKs changed from
--      CASCADE / SET NULL to RESTRICT — projects and users
--      must be deactivated, not deleted, preserving history.
--   9. "DailyProgressPhotos": optional "AreaID"/"ActivityID"
--      links + "SortOrder".
--  10. Extra reporting indexes.
-- ============================================================

CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- ============================================================
-- ENUM TYPES
-- ============================================================

CREATE TYPE entry_status AS ENUM ('DRAFT', 'SUBMITTED', 'APPROVED', 'REJECTED');
CREATE TYPE plan_type    AS ENUM ('DAILY', 'WEEKLY', 'MONTHLY');
CREATE TYPE shift_type   AS ENUM ('MORNING', 'EVENING', 'NIGHT');
CREATE TYPE user_role    AS ENUM ('ADMIN', 'PROJECT_MANAGER', 'SITE_ENGINEER', 'VIEWER');

-- ============================================================
-- TRIGGER FUNCTIONS
-- ============================================================

-- Auto-update "LastModifiedDate" on every UPDATE
CREATE OR REPLACE FUNCTION trg_set_last_modified()
RETURNS TRIGGER AS $$
BEGIN
    NEW."LastModifiedDate" := now();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Block edits to APPROVED headers.
-- To modify an approved record, the application must first move
-- the status away from APPROVED (e.g. back to DRAFT) in a
-- dedicated "unapprove" action — that status change itself is
-- allowed; any other edit while APPROVED is rejected.
CREATE OR REPLACE FUNCTION trg_block_approved_edit()
RETURNS TRIGGER AS $$
BEGIN
    IF OLD."Status" = 'APPROVED' AND NEW."Status" = 'APPROVED' THEN
        RAISE EXCEPTION 'Record % is APPROVED and cannot be modified. Un-approve it first.', OLD."ID"
            USING ERRCODE = 'check_violation';
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Keep "DailyProgress"."TotalAmount" = SUM of active detail amounts
CREATE OR REPLACE FUNCTION trg_sync_dpr_total()
RETURNS TRIGGER AS $$
DECLARE
    v_header_id INT;
BEGIN
    v_header_id := COALESCE(NEW."DailyProgressID", OLD."DailyProgressID");

    UPDATE "DailyProgress" dp
    SET "TotalAmount" = COALESCE((
            SELECT SUM(d."Amount")
            FROM "DailyProgressDetails" d
            WHERE d."DailyProgressID" = v_header_id
              AND d."IsActive"
        ), 0)
    WHERE dp."ID" = v_header_id;

    RETURN COALESCE(NEW, OLD);
END;
$$ LANGUAGE plpgsql;

-- ============================================================
-- 1. MASTERS
-- ============================================================

-- Unit of Measure master (SQM, MT, CUM, RMT, NOS...)
CREATE TABLE "UOM" (
    "ID"                INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "UniqueID"          UUID NOT NULL DEFAULT gen_random_uuid() UNIQUE,
    "CompanyID"         INT NOT NULL REFERENCES "Company"("ID") ON DELETE RESTRICT,
    "Name"              VARCHAR(100) NOT NULL,      -- e.g. 'Square Metre'
    "Code"              VARCHAR(20)  NOT NULL,      -- e.g. 'SQM'
    "IsActive"          BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedBy"         INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "CreatedDate"       TIMESTAMPTZ NOT NULL DEFAULT now(),
    "LastModifiedBy"    INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "LastModifiedDate"  TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE "Area" (
    "ID"                INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "UniqueID"          UUID NOT NULL DEFAULT gen_random_uuid() UNIQUE,
    "ProjectID"         INT NOT NULL REFERENCES "Project"("ID") ON DELETE RESTRICT,
    "Name"              VARCHAR(150) NOT NULL,      -- e.g. 'A1 Foundation'
    "Code"              VARCHAR(30),                -- e.g. 'A1'
    "IsActive"          BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedBy"         INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "CreatedDate"       TIMESTAMPTZ NOT NULL DEFAULT now(),
    "LastModifiedBy"    INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "LastModifiedDate"  TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE "Activity" (
    "ID"                INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "UniqueID"          UUID NOT NULL DEFAULT gen_random_uuid() UNIQUE,
    "CompanyID"         INT NOT NULL REFERENCES "Company"("ID") ON DELETE RESTRICT,
    "Name"              VARCHAR(150) NOT NULL,      -- Shuttering, Rebar / Steel Fixing...
    "UOMID"             INT NOT NULL REFERENCES "UOM"("ID") ON DELETE RESTRICT,
    "IsActive"          BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedBy"         INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "CreatedDate"       TIMESTAMPTZ NOT NULL DEFAULT now(),
    "LastModifiedBy"    INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "LastModifiedDate"  TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- "Enable Activities" toggle screen
-- NOTE: "IsActive" IS the toggle here (row active = activity
-- enabled for the project). The old separate "Enabled" column
-- was redundant and has been removed.
CREATE TABLE "ProjectActivity" (
    "ID"                INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "UniqueID"          UUID NOT NULL DEFAULT gen_random_uuid() UNIQUE,
    "ProjectID"         INT NOT NULL REFERENCES "Project"("ID") ON DELETE RESTRICT,
    "ActivityID"        INT NOT NULL REFERENCES "Activity"("ID") ON DELETE CASCADE,
    "IsActive"          BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedBy"         INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "CreatedDate"       TIMESTAMPTZ NOT NULL DEFAULT now(),
    "LastModifiedBy"    INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "LastModifiedDate"  TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- Rate Master screen: rate per activity per project
-- "UOMID" is a deliberate snapshot; normally equals the
-- activity's UOM but is FK-guaranteed valid either way.
CREATE TABLE "RateMaster" (
    "ID"                INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "UniqueID"          UUID NOT NULL DEFAULT gen_random_uuid() UNIQUE,
    "ProjectID"         INT NOT NULL REFERENCES "Project"("ID") ON DELETE RESTRICT,
    "ActivityID"        INT NOT NULL REFERENCES "Activity"("ID") ON DELETE RESTRICT,
    "Rate"              NUMERIC(12,2) NOT NULL CHECK ("Rate" >= 0),
    "UOMID"             INT NOT NULL REFERENCES "UOM"("ID") ON DELETE RESTRICT,
    "EffectiveFrom"     DATE NOT NULL DEFAULT CURRENT_DATE,
    "IsActive"          BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedBy"         INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "CreatedDate"       TIMESTAMPTZ NOT NULL DEFAULT now(),
    "LastModifiedBy"    INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "LastModifiedDate"  TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- ============================================================
-- 2. PLANNING  (Header + Details, dates in header)
-- ============================================================

CREATE TABLE "Planning" (
    "ID"                INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "UniqueID"          UUID NOT NULL DEFAULT gen_random_uuid() UNIQUE,
    "ProjectID"         INT NOT NULL REFERENCES "Project"("ID") ON DELETE RESTRICT,
    "PlanType"          plan_type NOT NULL DEFAULT 'DAILY',
    "StartDate"         DATE NOT NULL,
    "EndDate"           DATE,
    "Remarks"           TEXT,
    "Status"            entry_status NOT NULL DEFAULT 'DRAFT',
    "ApprovedBy"        INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "ApprovedDate"      TIMESTAMPTZ,
    "RejectionReason"   TEXT,
    "IsActive"          BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedBy"         INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "CreatedDate"       TIMESTAMPTZ NOT NULL DEFAULT now(),
    "LastModifiedBy"    INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "LastModifiedDate"  TIMESTAMPTZ NOT NULL DEFAULT now(),
    CONSTRAINT "CHK_Planning_Dates" CHECK ("EndDate" IS NULL OR "EndDate" >= "StartDate")
);

CREATE TABLE "PlanningDetails" (
    "ID"                INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "UniqueID"          UUID NOT NULL DEFAULT gen_random_uuid() UNIQUE,
    "PlanningID"        INT NOT NULL REFERENCES "Planning"("ID") ON DELETE CASCADE,
    "AreaID"            INT NOT NULL REFERENCES "Area"("ID") ON DELETE RESTRICT,
    "ActivityID"        INT NOT NULL REFERENCES "Activity"("ID") ON DELETE RESTRICT,
    "TargetQuantity"    NUMERIC(14,3) NOT NULL DEFAULT 0 CHECK ("TargetQuantity" >= 0),
    "UOMID"             INT NOT NULL REFERENCES "UOM"("ID") ON DELETE RESTRICT,
    "Remarks"           TEXT,
    "IsActive"          BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedBy"         INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "CreatedDate"       TIMESTAMPTZ NOT NULL DEFAULT now(),
    "LastModifiedBy"    INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "LastModifiedDate"  TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- ============================================================
-- 3. MANPOWER  (Header + Details, date in header)
-- ============================================================

CREATE TABLE "Manpower" (
    "ID"                INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "UniqueID"          UUID NOT NULL DEFAULT gen_random_uuid() UNIQUE,
    "ProjectID"         INT NOT NULL REFERENCES "Project"("ID") ON DELETE RESTRICT,
    "EntryDate"         DATE NOT NULL,
    "Shift"             shift_type NOT NULL DEFAULT 'MORNING',
    "Remarks"           TEXT,
    "Status"            entry_status NOT NULL DEFAULT 'DRAFT',
    "ApprovedBy"        INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "ApprovedDate"      TIMESTAMPTZ,
    "RejectionReason"   TEXT,
    "IsActive"          BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedBy"         INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "CreatedDate"       TIMESTAMPTZ NOT NULL DEFAULT now(),
    "LastModifiedBy"    INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "LastModifiedDate"  TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE "ManpowerDetails" (
    "ID"                INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "UniqueID"          UUID NOT NULL DEFAULT gen_random_uuid() UNIQUE,
    "ManpowerID"        INT NOT NULL REFERENCES "Manpower"("ID") ON DELETE CASCADE,
    "AreaID"            INT NOT NULL REFERENCES "Area"("ID") ON DELETE RESTRICT,
    "ContractorID"      INT NOT NULL REFERENCES "Contractor"("ID") ON DELETE RESTRICT,
    "ActivityID"        INT NOT NULL REFERENCES "Activity"("ID") ON DELETE RESTRICT,
    "SkilledCount"      INT NOT NULL DEFAULT 0 CHECK ("SkilledCount" >= 0),
    "UnskilledCount"    INT NOT NULL DEFAULT 0 CHECK ("UnskilledCount" >= 0),
    "OtherCount"        INT NOT NULL DEFAULT 0 CHECK ("OtherCount" >= 0),
    "TotalCount"        INT GENERATED ALWAYS AS
                        ("SkilledCount" + "UnskilledCount" + "OtherCount") STORED,
    "IsActive"          BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedBy"         INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "CreatedDate"       TIMESTAMPTZ NOT NULL DEFAULT now(),
    "LastModifiedBy"    INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "LastModifiedDate"  TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- ============================================================
-- 4. DAILY PROGRESS (DPR)  (Header + Details)
-- ============================================================

CREATE TABLE "DailyProgress" (
    "ID"                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "UniqueID"            UUID NOT NULL DEFAULT gen_random_uuid() UNIQUE,
    "ProjectID"           INT NOT NULL REFERENCES "Project"("ID") ON DELETE RESTRICT,
    "ReportDate"          DATE NOT NULL,
    "Hindrances"          TEXT,
    "HindranceAudioUrl"   TEXT,
    "NextDayPlan"         TEXT,
    "Remarks"             TEXT,
    -- Maintained automatically by trg_sync_dpr_total on details
    "TotalAmount"         NUMERIC(14,2) NOT NULL DEFAULT 0,
    "Status"              entry_status NOT NULL DEFAULT 'DRAFT',
    "ApprovedBy"          INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "ApprovedDate"        TIMESTAMPTZ,
    "RejectionReason"     TEXT,
    "IsActive"            BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedBy"           INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "CreatedDate"         TIMESTAMPTZ NOT NULL DEFAULT now(),
    "LastModifiedBy"      INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "LastModifiedDate"    TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- Area-wise DPR: "AreaID" added so progress lines up with
-- PlanningDetails / ManpowerDetails granularity.
CREATE TABLE "DailyProgressDetails" (
    "ID"                INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "UniqueID"          UUID NOT NULL DEFAULT gen_random_uuid() UNIQUE,
    "DailyProgressID"   INT NOT NULL REFERENCES "DailyProgress"("ID") ON DELETE CASCADE,
    "AreaID"            INT NOT NULL REFERENCES "Area"("ID") ON DELETE RESTRICT,
    "ActivityID"        INT NOT NULL REFERENCES "Activity"("ID") ON DELETE RESTRICT,
    "Quantity"          NUMERIC(14,3) NOT NULL DEFAULT 0 CHECK ("Quantity" >= 0),
    "UOMID"             INT NOT NULL REFERENCES "UOM"("ID") ON DELETE RESTRICT,
    "Rate"              NUMERIC(12,2) NOT NULL DEFAULT 0,   -- snapshot from RateMaster
    "Amount"            NUMERIC(14,2) GENERATED ALWAYS AS ("Quantity" * "Rate") STORED,
    "PlanQuantity"      NUMERIC(14,3),                      -- snapshot from PlanningDetails
    "Variance"          NUMERIC(14,3) GENERATED ALWAYS AS
                        ("Quantity" - COALESCE("PlanQuantity", 0)) STORED,
    "Remarks"           TEXT,
    "IsActive"          BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedBy"         INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "CreatedDate"       TIMESTAMPTZ NOT NULL DEFAULT now(),
    "LastModifiedBy"    INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "LastModifiedDate"  TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE "DailyProgressPhotos" (
    "ID"                INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "UniqueID"          UUID NOT NULL DEFAULT gen_random_uuid() UNIQUE,
    "DailyProgressID"   INT NOT NULL REFERENCES "DailyProgress"("ID") ON DELETE CASCADE,
    "AreaID"            INT REFERENCES "Area"("ID") ON DELETE SET NULL,      -- optional tag
    "ActivityID"        INT REFERENCES "Activity"("ID") ON DELETE SET NULL,  -- optional tag
    "PhotoUrl"          TEXT NOT NULL,
    "Caption"           VARCHAR(300),
    "SortOrder"         INT NOT NULL DEFAULT 0,
    "IsActive"          BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedBy"         INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "CreatedDate"       TIMESTAMPTZ NOT NULL DEFAULT now(),
    "LastModifiedBy"    INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "LastModifiedDate"  TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- ============================================================
-- 5. DAILY LABOR  (Header + Details, auto-filled from Manpower, editable)
-- ============================================================

CREATE TABLE "DailyLabor" (
    "ID"                INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "UniqueID"          UUID NOT NULL DEFAULT gen_random_uuid() UNIQUE,
    "ProjectID"         INT NOT NULL REFERENCES "Project"("ID") ON DELETE RESTRICT,
    "ReportDate"        DATE NOT NULL,
    "Shift"             shift_type NOT NULL DEFAULT 'MORNING',   -- mirrors Manpower
    "Remarks"           TEXT,
    "Status"            entry_status NOT NULL DEFAULT 'DRAFT',
    "ApprovedBy"        INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "ApprovedDate"      TIMESTAMPTZ,
    "RejectionReason"   TEXT,
    "IsActive"          BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedBy"         INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "CreatedDate"       TIMESTAMPTZ NOT NULL DEFAULT now(),
    "LastModifiedBy"    INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "LastModifiedDate"  TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE "DailyLaborDetails" (
    "ID"                INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "UniqueID"          UUID NOT NULL DEFAULT gen_random_uuid() UNIQUE,
    "DailyLaborID"      INT NOT NULL REFERENCES "DailyLabor"("ID") ON DELETE CASCADE,
    "ContractorID"      INT NOT NULL REFERENCES "Contractor"("ID") ON DELETE RESTRICT,
    "ActivityID"        INT NOT NULL REFERENCES "Activity"("ID") ON DELETE RESTRICT,
    "SkilledCount"      INT NOT NULL DEFAULT 0 CHECK ("SkilledCount" >= 0),
    "UnskilledCount"    INT NOT NULL DEFAULT 0 CHECK ("UnskilledCount" >= 0),
    "OtherCount"        INT NOT NULL DEFAULT 0 CHECK ("OtherCount" >= 0),  -- was "MatCount"
    "TotalCount"        INT GENERATED ALWAYS AS
                        ("SkilledCount" + "UnskilledCount" + "OtherCount") STORED,
    "IsActive"          BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedBy"         INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "CreatedDate"       TIMESTAMPTZ NOT NULL DEFAULT now(),
    "LastModifiedBy"    INT REFERENCES "Users"("ID") ON DELETE RESTRICT,
    "LastModifiedDate"  TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- ============================================================
-- 6. BUSINESS UNIQUE RULES — partial indexes (soft-delete aware)
--    Only ACTIVE rows participate, so soft-deleting a row frees
--    the name/date/combination for re-use.
-- ============================================================

CREATE UNIQUE INDEX "UQ_UOM_Code_Per_Company"
    ON "UOM"("CompanyID", "Code") WHERE "IsActive";

CREATE UNIQUE INDEX "UQ_Area_Per_Project"
    ON "Area"("ProjectID", "Name") WHERE "IsActive";

CREATE UNIQUE INDEX "UQ_Activity_Per_Company"
    ON "Activity"("CompanyID", "Name") WHERE "IsActive";

CREATE UNIQUE INDEX "UQ_ProjectActivity"
    ON "ProjectActivity"("ProjectID", "ActivityID") WHERE "IsActive";

CREATE UNIQUE INDEX "UQ_Rate_Per_Activity"
    ON "RateMaster"("ProjectID", "ActivityID", "EffectiveFrom") WHERE "IsActive";

CREATE UNIQUE INDEX "UQ_PlanningDetails"
    ON "PlanningDetails"("PlanningID", "AreaID", "ActivityID") WHERE "IsActive";

-- one header per project per date per shift
CREATE UNIQUE INDEX "UQ_Manpower_Header"
    ON "Manpower"("ProjectID", "EntryDate", "Shift") WHERE "IsActive";

CREATE UNIQUE INDEX "UQ_ManpowerDetails"
    ON "ManpowerDetails"("ManpowerID", "AreaID", "ContractorID", "ActivityID") WHERE "IsActive";

-- one DPR per project per date
CREATE UNIQUE INDEX "UQ_DailyProgress_Per_Date"
    ON "DailyProgress"("ProjectID", "ReportDate") WHERE "IsActive";

-- no duplicate progress lines per header
CREATE UNIQUE INDEX "UQ_DailyProgressDetails"
    ON "DailyProgressDetails"("DailyProgressID", "AreaID", "ActivityID") WHERE "IsActive";

-- one labor report per project per date per shift
CREATE UNIQUE INDEX "UQ_DailyLabor_Per_Date"
    ON "DailyLabor"("ProjectID", "ReportDate", "Shift") WHERE "IsActive";

CREATE UNIQUE INDEX "UQ_DailyLaborDetails"
    ON "DailyLaborDetails"("DailyLaborID", "ContractorID", "ActivityID") WHERE "IsActive";

-- ============================================================
-- 7. ATTACH TRIGGERS
-- ============================================================

-- 7a. LastModifiedDate trigger on all tables
DO $$
DECLARE
    t TEXT;
BEGIN
    FOREACH t IN ARRAY ARRAY[
        'Company','Users','Project','UOM','Area','Activity','ProjectActivity',
        'Contractor','RateMaster','Planning','PlanningDetails',
        'Manpower','ManpowerDetails','DailyProgress','DailyProgressDetails',
        'DailyProgressPhotos','DailyLabor','DailyLaborDetails'
    ]
    LOOP
        EXECUTE format(
            'CREATE TRIGGER trg_%s_modified BEFORE UPDATE ON %I
             FOR EACH ROW EXECUTE FUNCTION trg_set_last_modified();',
            lower(t), t
        );
    END LOOP;
END $$;

-- 7b. Block edits to APPROVED headers
DO $$
DECLARE
    t TEXT;
BEGIN
    FOREACH t IN ARRAY ARRAY[
        'Planning','Manpower','DailyProgress','DailyLabor'
    ]
    LOOP
        EXECUTE format(
            'CREATE TRIGGER trg_%s_block_approved BEFORE UPDATE ON %I
             FOR EACH ROW EXECUTE FUNCTION trg_block_approved_edit();',
            lower(t), t
        );
    END LOOP;
END $$;

-- 7c. Keep DailyProgress.TotalAmount in sync with detail amounts
CREATE TRIGGER trg_dpdetails_sync_total
    AFTER INSERT OR UPDATE OR DELETE ON "DailyProgressDetails"
    FOR EACH ROW EXECUTE FUNCTION trg_sync_dpr_total();

-- ============================================================
-- 8. INDEXES (FK lookup + common filters)
-- ============================================================

CREATE INDEX "IDX_UOM_Company"              ON "UOM"("CompanyID");
CREATE INDEX "IDX_Area_Project"             ON "Area"("ProjectID");
CREATE INDEX "IDX_Activity_Company"         ON "Activity"("CompanyID");
CREATE INDEX "IDX_Activity_UOM"             ON "Activity"("UOMID");
CREATE INDEX "IDX_Contractor_Company"       ON "Contractor"("CompanyID");
CREATE INDEX "IDX_RateMaster_Lookup"        ON "RateMaster"("ProjectID", "ActivityID", "EffectiveFrom" DESC);

CREATE INDEX "IDX_Planning_Project"         ON "Planning"("ProjectID", "StartDate", "EndDate");
CREATE INDEX "IDX_PlanningDetails_Header"   ON "PlanningDetails"("PlanningID");
CREATE INDEX "IDX_PlanningDetails_Activity" ON "PlanningDetails"("ActivityID", "AreaID");

CREATE INDEX "IDX_Manpower_Date"            ON "Manpower"("ProjectID", "EntryDate");
CREATE INDEX "IDX_ManpowerDetails_Header"   ON "ManpowerDetails"("ManpowerID");
CREATE INDEX "IDX_ManpowerDetails_Refs"     ON "ManpowerDetails"("ContractorID", "ActivityID");

CREATE INDEX "IDX_DailyProgress_Date"       ON "DailyProgress"("ProjectID", "ReportDate" DESC);
CREATE INDEX "IDX_DPDetails_Header"         ON "DailyProgressDetails"("DailyProgressID");
CREATE INDEX "IDX_DPDetails_Activity"       ON "DailyProgressDetails"("ActivityID", "AreaID");
CREATE INDEX "IDX_DPPhotos_Header"          ON "DailyProgressPhotos"("DailyProgressID");

CREATE INDEX "IDX_DailyLabor_Date"          ON "DailyLabor"("ProjectID", "ReportDate" DESC);
CREATE INDEX "IDX_DailyLaborDetails_Header" ON "DailyLaborDetails"("DailyLaborID");
CREATE INDEX "IDX_DailyLaborDetails_Refs"   ON "DailyLaborDetails"("ContractorID", "ActivityID");
