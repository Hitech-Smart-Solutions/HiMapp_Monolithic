using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Himapp.PM.Infrastructure.Migrations.PM
{
    /// <summary>
    /// Creates the PostgreSQL functions behind the Dynamics equipment category and master list endpoints
    /// (paging, search, filter on one column, sort). No table changes.
    /// </summary>
    public partial class AddEquipmentListFunctions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
-- Returns the real (correctly cased) column name of a table when it exists, otherwise NULL.
-- Used to whitelist sort and filter columns so that no user text is ever placed into SQL unchecked.
CREATE OR REPLACE FUNCTION "PAndM".fn_resolve_column(p_table regclass, p_column text)
RETURNS text
LANGUAGE sql
STABLE
AS $$
    SELECT a.attname::text
    FROM pg_attribute a
    WHERE a.attrelid = p_table
      AND a.attnum > 0
      AND NOT a.attisdropped
      AND lower(a.attname) = lower(btrim(COALESCE(p_column, '')))
    LIMIT 1;
$$;

-- Turns the text typed by the user into a safe "contains" pattern for ILIKE.
CREATE OR REPLACE FUNCTION "PAndM".fn_like_pattern(p_value text)
RETURNS text
LANGUAGE sql
IMMUTABLE
AS $$
    SELECT '%' || regexp_replace(btrim(COALESCE(p_value, '')), '([\\%_])', '\\\1', 'g') || '%';
$$;

-- Builds the WHERE clause (value is always passed as parameter $1).
--   filter column given  -> that single column contains the value
--   no filter column     -> any of the default search columns contains the value
--   no value             -> empty string (no filtering)
CREATE OR REPLACE FUNCTION "PAndM".fn_build_list_where(
    p_table          regclass,
    p_filtercolumn   text,
    p_filtervalue    text,
    p_searchcolumns  text[])
RETURNS text
LANGUAGE plpgsql
STABLE
AS $$
DECLARE
    v_column text;
    v_parts  text[] := ARRAY[]::text[];
    v_name   text;
BEGIN
    IF btrim(COALESCE(p_filtervalue, '')) = '' THEN
        RETURN '';
    END IF;

    IF btrim(COALESCE(p_filtercolumn, '')) <> '' THEN
        v_column := "PAndM".fn_resolve_column(p_table, p_filtercolumn);
        IF v_column IS NULL THEN
            RAISE EXCEPTION 'Invalid filter column: %', p_filtercolumn USING ERRCODE = '22023';
        END IF;
        RETURN format('WHERE t.%I::text ILIKE $1', v_column);
    END IF;

    FOREACH v_name IN ARRAY p_searchcolumns LOOP
        v_parts := v_parts || format('t.%I::text ILIKE $1', v_name);
    END LOOP;

    RETURN 'WHERE (' || array_to_string(v_parts, ' OR ') || ')';
END;
$$;
""");

            migrationBuilder.Sql("""
CREATE OR REPLACE FUNCTION "PAndM".uspgetdynamicsequipmentcategories(
    p_filtercolumn  text    DEFAULT NULL,
    p_filtervalue   text    DEFAULT NULL,
    p_pageindex     integer DEFAULT 1,
    p_pagesize      integer DEFAULT 25,
    p_sortcolumn    text    DEFAULT NULL,
    p_sortdirection text    DEFAULT 'asc')
RETURNS SETOF "PAndM"."DynamicsEquipmentCategory"
LANGUAGE plpgsql
STABLE
AS $$
DECLARE
    v_table   regclass := '"PAndM"."DynamicsEquipmentCategory"'::regclass;
    v_where   text;
    v_sortcol text;
    v_dir     text;
    v_size    integer;
    v_page    integer;
BEGIN
    v_where   := "PAndM".fn_build_list_where(v_table, p_filtercolumn, p_filtervalue, ARRAY['DataAreaId','OBJECTTYPEID','NAME','OBJECTLIFECYCLEMODELID','SHORTNAME_ISPL']);
    v_sortcol := COALESCE("PAndM".fn_resolve_column(v_table, p_sortcolumn), 'ID');
    v_dir     := CASE WHEN lower(btrim(COALESCE(p_sortdirection, ''))) = 'desc' THEN 'DESC' ELSE 'ASC' END;
    v_size    := LEAST(GREATEST(COALESCE(p_pagesize, 25), 1), 200);
    v_page    := GREATEST(COALESCE(p_pageindex, 1), 1);

    RETURN QUERY EXECUTE format(
        'SELECT t.* FROM %s t %s ORDER BY t.%I %s, t."ID" LIMIT %s OFFSET %s',
        v_table, v_where, v_sortcol, v_dir, v_size, (v_page - 1) * v_size)
    USING "PAndM".fn_like_pattern(p_filtervalue);
END;
$$;

CREATE OR REPLACE FUNCTION "PAndM".uspgetdynamicsequipmentcategoriescount(
    p_filtercolumn  text DEFAULT NULL,
    p_filtervalue   text DEFAULT NULL)
RETURNS TABLE (cnt bigint)
LANGUAGE plpgsql
STABLE
AS $$
DECLARE
    v_table regclass := '"PAndM"."DynamicsEquipmentCategory"'::regclass;
    v_where text;
BEGIN
    v_where := "PAndM".fn_build_list_where(v_table, p_filtercolumn, p_filtervalue, ARRAY['DataAreaId','OBJECTTYPEID','NAME','OBJECTLIFECYCLEMODELID','SHORTNAME_ISPL']);

    RETURN QUERY EXECUTE format('SELECT count(*)::bigint FROM %s t %s', v_table, v_where)
    USING "PAndM".fn_like_pattern(p_filtervalue);
END;
$$;
""");

            migrationBuilder.Sql("""
CREATE OR REPLACE FUNCTION "PAndM".uspgetdynamicsequipmentmasters(
    p_filtercolumn  text    DEFAULT NULL,
    p_filtervalue   text    DEFAULT NULL,
    p_pageindex     integer DEFAULT 1,
    p_pagesize      integer DEFAULT 25,
    p_sortcolumn    text    DEFAULT NULL,
    p_sortdirection text    DEFAULT 'asc')
RETURNS SETOF "PAndM"."DynamicsEquipmentMaster"
LANGUAGE plpgsql
STABLE
AS $$
DECLARE
    v_table   regclass := '"PAndM"."DynamicsEquipmentMaster"'::regclass;
    v_where   text;
    v_sortcol text;
    v_dir     text;
    v_size    integer;
    v_page    integer;
BEGIN
    v_where   := "PAndM".fn_build_list_where(v_table, p_filtercolumn, p_filtervalue, ARRAY['OBJECTID','NAME','FIXEDASSETID','ISPLVEHICLENUMBER','SERIALID','MANUFACTURER','MODEL','OBJECTTYPE']);
    v_sortcol := COALESCE("PAndM".fn_resolve_column(v_table, p_sortcolumn), 'ID');
    v_dir     := CASE WHEN lower(btrim(COALESCE(p_sortdirection, ''))) = 'desc' THEN 'DESC' ELSE 'ASC' END;
    v_size    := LEAST(GREATEST(COALESCE(p_pagesize, 25), 1), 200);
    v_page    := GREATEST(COALESCE(p_pageindex, 1), 1);

    RETURN QUERY EXECUTE format(
        'SELECT t.* FROM %s t %s ORDER BY t.%I %s, t."ID" LIMIT %s OFFSET %s',
        v_table, v_where, v_sortcol, v_dir, v_size, (v_page - 1) * v_size)
    USING "PAndM".fn_like_pattern(p_filtervalue);
END;
$$;

CREATE OR REPLACE FUNCTION "PAndM".uspgetdynamicsequipmentmasterscount(
    p_filtercolumn  text DEFAULT NULL,
    p_filtervalue   text DEFAULT NULL)
RETURNS TABLE (cnt bigint)
LANGUAGE plpgsql
STABLE
AS $$
DECLARE
    v_table regclass := '"PAndM"."DynamicsEquipmentMaster"'::regclass;
    v_where text;
BEGIN
    v_where := "PAndM".fn_build_list_where(v_table, p_filtercolumn, p_filtervalue, ARRAY['OBJECTID','NAME','FIXEDASSETID','ISPLVEHICLENUMBER','SERIALID','MANUFACTURER','MODEL','OBJECTTYPE']);

    RETURN QUERY EXECUTE format('SELECT count(*)::bigint FROM %s t %s', v_table, v_where)
    USING "PAndM".fn_like_pattern(p_filtervalue);
END;
$$;
""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
DROP FUNCTION IF EXISTS "PAndM".uspgetdynamicsequipmentmasterscount(text, text);
DROP FUNCTION IF EXISTS "PAndM".uspgetdynamicsequipmentmasters(text, text, integer, integer, text, text);
DROP FUNCTION IF EXISTS "PAndM".uspgetdynamicsequipmentcategoriescount(text, text);
DROP FUNCTION IF EXISTS "PAndM".uspgetdynamicsequipmentcategories(text, text, integer, integer, text, text);
DROP FUNCTION IF EXISTS "PAndM".fn_build_list_where(regclass, text, text, text[]);
DROP FUNCTION IF EXISTS "PAndM".fn_like_pattern(text);
DROP FUNCTION IF EXISTS "PAndM".fn_resolve_column(regclass, text);
""");
        }
    }
}
