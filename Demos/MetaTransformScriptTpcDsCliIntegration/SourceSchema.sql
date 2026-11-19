-- Source table declarations reconstructed from the previous dbo MetaSchema contract, including modeled facets.

-- These declarations are the explicit, reproducible SQL input to fixture extraction.

EXEC(N'CREATE SCHEMA tpcds');

CREATE TABLE dbo.[call_center] (
    [cc_call_center_sk] int NOT NULL,
    [cc_call_center_id] char(16) NOT NULL,
    [cc_rec_start_date] date NULL,
    [cc_rec_end_date] date NULL,
    [cc_closed_date_sk] int NULL,
    [cc_open_date_sk] int NULL,
    [cc_name] varchar(50) NULL,
    [cc_class] varchar(50) NULL,
    [cc_employees] int NULL,
    [cc_sq_ft] int NULL,
    [cc_hours] char(20) NULL,
    [cc_manager] varchar(40) NULL,
    [cc_mkt_id] int NULL,
    [cc_mkt_class] char(50) NULL,
    [cc_mkt_desc] varchar(100) NULL,
    [cc_market_manager] varchar(40) NULL,
    [cc_division] int NULL,
    [cc_division_name] varchar(50) NULL,
    [cc_company] int NULL,
    [cc_company_name] char(50) NULL,
    [cc_street_number] char(10) NULL,
    [cc_street_name] varchar(60) NULL,
    [cc_street_type] char(15) NULL,
    [cc_suite_number] char(10) NULL,
    [cc_city] varchar(60) NULL,
    [cc_county] varchar(30) NULL,
    [cc_state] char(2) NULL,
    [cc_zip] char(10) NULL,
    [cc_country] varchar(20) NULL,
    [cc_gmt_offset] decimal(5,2) NULL,
    [cc_tax_percentage] decimal(5,2) NULL
);

CREATE TABLE dbo.[catalog_page] (
    [cp_catalog_page_sk] int NOT NULL,
    [cp_catalog_page_id] char(16) NOT NULL,
    [cp_start_date_sk] int NULL,
    [cp_end_date_sk] int NULL,
    [cp_department] varchar(50) NULL,
    [cp_catalog_number] int NULL,
    [cp_catalog_page_number] int NULL,
    [cp_description] varchar(100) NULL,
    [cp_type] varchar(100) NULL
);

CREATE TABLE dbo.[catalog_returns] (
    [cr_returned_date_sk] int NULL,
    [cr_returned_time_sk] int NULL,
    [cr_item_sk] int NOT NULL,
    [cr_refunded_customer_sk] int NULL,
    [cr_refunded_cdemo_sk] int NULL,
    [cr_refunded_hdemo_sk] int NULL,
    [cr_refunded_addr_sk] int NULL,
    [cr_returning_customer_sk] int NULL,
    [cr_returning_cdemo_sk] int NULL,
    [cr_returning_hdemo_sk] int NULL,
    [cr_returning_addr_sk] int NULL,
    [cr_call_center_sk] int NULL,
    [cr_catalog_page_sk] int NULL,
    [cr_ship_mode_sk] int NULL,
    [cr_warehouse_sk] int NULL,
    [cr_reason_sk] int NULL,
    [cr_order_number] int NOT NULL,
    [cr_return_quantity] int NULL,
    [cr_return_amount] decimal(7,2) NULL,
    [cr_return_tax] decimal(7,2) NULL,
    [cr_return_amt_inc_tax] decimal(7,2) NULL,
    [cr_fee] decimal(7,2) NULL,
    [cr_return_ship_cost] decimal(7,2) NULL,
    [cr_refunded_cash] decimal(7,2) NULL,
    [cr_reversed_charge] decimal(7,2) NULL,
    [cr_store_credit] decimal(7,2) NULL,
    [cr_net_loss] decimal(7,2) NULL
);

CREATE TABLE dbo.[catalog_sales] (
    [cs_sold_date_sk] int NULL,
    [cs_sold_time_sk] int NULL,
    [cs_ship_date_sk] int NULL,
    [cs_bill_customer_sk] int NULL,
    [cs_bill_cdemo_sk] int NULL,
    [cs_bill_hdemo_sk] int NULL,
    [cs_bill_addr_sk] int NULL,
    [cs_ship_customer_sk] int NULL,
    [cs_ship_cdemo_sk] int NULL,
    [cs_ship_hdemo_sk] int NULL,
    [cs_ship_addr_sk] int NULL,
    [cs_call_center_sk] int NULL,
    [cs_catalog_page_sk] int NULL,
    [cs_ship_mode_sk] int NULL,
    [cs_warehouse_sk] int NULL,
    [cs_item_sk] int NOT NULL,
    [cs_promo_sk] int NULL,
    [cs_order_number] int NOT NULL,
    [cs_quantity] int NULL,
    [cs_wholesale_cost] decimal(7,2) NULL,
    [cs_list_price] decimal(7,2) NULL,
    [cs_sales_price] decimal(7,2) NULL,
    [cs_ext_discount_amt] decimal(7,2) NULL,
    [cs_ext_sales_price] decimal(7,2) NULL,
    [cs_ext_wholesale_cost] decimal(7,2) NULL,
    [cs_ext_list_price] decimal(7,2) NULL,
    [cs_ext_tax] decimal(7,2) NULL,
    [cs_coupon_amt] decimal(7,2) NULL,
    [cs_ext_ship_cost] decimal(7,2) NULL,
    [cs_net_paid] decimal(7,2) NULL,
    [cs_net_paid_inc_tax] decimal(7,2) NULL,
    [cs_net_paid_inc_ship] decimal(7,2) NULL,
    [cs_net_paid_inc_ship_tax] decimal(7,2) NULL,
    [cs_net_profit] decimal(7,2) NULL
);

CREATE TABLE dbo.[customer] (
    [c_customer_sk] int NOT NULL,
    [c_customer_id] char(16) NOT NULL,
    [c_current_cdemo_sk] int NULL,
    [c_current_hdemo_sk] int NULL,
    [c_current_addr_sk] int NULL,
    [c_first_shipto_date_sk] int NULL,
    [c_first_sales_date_sk] int NULL,
    [c_salutation] char(10) NULL,
    [c_first_name] char(20) NULL,
    [c_last_name] char(30) NULL,
    [c_preferred_cust_flag] char(1) NULL,
    [c_birth_day] int NULL,
    [c_birth_month] int NULL,
    [c_birth_year] int NULL,
    [c_birth_country] varchar(20) NULL,
    [c_login] char(13) NULL,
    [c_email_address] char(50) NULL,
    [c_last_review_date] char(10) NULL
);

CREATE TABLE dbo.[customer_address] (
    [ca_address_sk] int NOT NULL,
    [ca_address_id] char(16) NOT NULL,
    [ca_street_number] char(10) NULL,
    [ca_street_name] varchar(60) NULL,
    [ca_street_type] char(15) NULL,
    [ca_suite_number] char(10) NULL,
    [ca_city] varchar(60) NULL,
    [ca_county] varchar(30) NULL,
    [ca_state] char(2) NULL,
    [ca_zip] char(10) NULL,
    [ca_country] varchar(20) NULL,
    [ca_gmt_offset] decimal(5,2) NULL,
    [ca_location_type] char(20) NULL
);

CREATE TABLE dbo.[customer_demographics] (
    [cd_demo_sk] int NOT NULL,
    [cd_gender] char(1) NULL,
    [cd_marital_status] char(1) NULL,
    [cd_education_status] char(20) NULL,
    [cd_purchase_estimate] int NULL,
    [cd_credit_rating] char(10) NULL,
    [cd_dep_count] int NULL,
    [cd_dep_employed_count] int NULL,
    [cd_dep_college_count] int NULL
);

CREATE TABLE dbo.[date_dim] (
    [d_date_sk] int NOT NULL,
    [d_date_id] char(16) NOT NULL,
    [d_date] date NULL,
    [d_month_seq] int NULL,
    [d_week_seq] int NULL,
    [d_quarter_seq] int NULL,
    [d_year] int NULL,
    [d_dow] int NULL,
    [d_moy] int NULL,
    [d_dom] int NULL,
    [d_qoy] int NULL,
    [d_fy_year] int NULL,
    [d_fy_quarter_seq] int NULL,
    [d_fy_week_seq] int NULL,
    [d_day_name] char(9) NULL,
    [d_quarter_name] char(6) NULL,
    [d_holiday] char(1) NULL,
    [d_weekend] char(1) NULL,
    [d_following_holiday] char(1) NULL,
    [d_first_dom] int NULL,
    [d_last_dom] int NULL,
    [d_same_day_ly] int NULL,
    [d_same_day_lq] int NULL,
    [d_current_day] char(1) NULL,
    [d_current_week] char(1) NULL,
    [d_current_month] char(1) NULL,
    [d_current_quarter] char(1) NULL,
    [d_current_year] char(1) NULL
);

CREATE TABLE dbo.[household_demographics] (
    [hd_demo_sk] int NOT NULL,
    [hd_income_band_sk] int NULL,
    [hd_buy_potential] char(15) NULL,
    [hd_dep_count] int NULL,
    [hd_vehicle_count] int NULL
);

CREATE TABLE dbo.[income_band] (
    [ib_income_band_sk] int NOT NULL,
    [ib_lower_bound] int NULL,
    [ib_upper_bound] int NULL
);

CREATE TABLE dbo.[inventory] (
    [inv_date_sk] int NOT NULL,
    [inv_item_sk] int NOT NULL,
    [inv_warehouse_sk] int NOT NULL,
    [inv_quantity_on_hand] int NULL
);

CREATE TABLE dbo.[item] (
    [i_item_sk] int NOT NULL,
    [i_item_id] char(16) NOT NULL,
    [i_rec_start_date] date NULL,
    [i_rec_end_date] date NULL,
    [i_item_desc] varchar(200) NULL,
    [i_current_price] decimal(7,2) NULL,
    [i_wholesale_cost] decimal(7,2) NULL,
    [i_brand_id] int NULL,
    [i_brand] char(50) NULL,
    [i_class_id] int NULL,
    [i_class] char(50) NULL,
    [i_category_id] int NULL,
    [i_category] char(50) NULL,
    [i_manufact_id] int NULL,
    [i_manufact] char(50) NULL,
    [i_size] char(20) NULL,
    [i_formulation] char(20) NULL,
    [i_color] char(20) NULL,
    [i_units] char(10) NULL,
    [i_container] char(10) NULL,
    [i_manager_id] int NULL,
    [i_product_name] char(50) NULL
);

CREATE TABLE dbo.[promotion] (
    [p_promo_sk] int NOT NULL,
    [p_promo_id] char(16) NOT NULL,
    [p_start_date_sk] int NULL,
    [p_end_date_sk] int NULL,
    [p_item_sk] int NULL,
    [p_cost] decimal(15,2) NULL,
    [p_response_target] int NULL,
    [p_promo_name] char(50) NULL,
    [p_channel_dmail] char(1) NULL,
    [p_channel_email] char(1) NULL,
    [p_channel_catalog] char(1) NULL,
    [p_channel_tv] char(1) NULL,
    [p_channel_radio] char(1) NULL,
    [p_channel_press] char(1) NULL,
    [p_channel_event] char(1) NULL,
    [p_channel_demo] char(1) NULL,
    [p_channel_details] varchar(100) NULL,
    [p_purpose] char(15) NULL,
    [p_discount_active] char(1) NULL
);

CREATE TABLE dbo.[reason] (
    [r_reason_sk] int NOT NULL,
    [r_reason_id] char(16) NOT NULL,
    [r_reason_desc] char(100) NULL
);

CREATE TABLE dbo.[ship_mode] (
    [sm_ship_mode_sk] int NOT NULL,
    [sm_ship_mode_id] char(16) NOT NULL,
    [sm_type] char(30) NULL,
    [sm_code] char(10) NULL,
    [sm_carrier] char(20) NULL,
    [sm_contract] char(20) NULL
);

CREATE TABLE dbo.[store] (
    [s_store_sk] int NOT NULL,
    [s_store_id] char(16) NOT NULL,
    [s_rec_start_date] date NULL,
    [s_rec_end_date] date NULL,
    [s_closed_date_sk] int NULL,
    [s_store_name] varchar(50) NULL,
    [s_number_employees] int NULL,
    [s_floor_space] int NULL,
    [s_hours] char(20) NULL,
    [s_manager] varchar(40) NULL,
    [s_market_id] int NULL,
    [s_geography_class] varchar(100) NULL,
    [s_market_desc] varchar(100) NULL,
    [s_market_manager] varchar(40) NULL,
    [s_division_id] int NULL,
    [s_division_name] varchar(50) NULL,
    [s_company_id] int NULL,
    [s_company_name] varchar(50) NULL,
    [s_street_number] varchar(10) NULL,
    [s_street_name] varchar(60) NULL,
    [s_street_type] char(15) NULL,
    [s_suite_number] char(10) NULL,
    [s_city] varchar(60) NULL,
    [s_county] varchar(30) NULL,
    [s_state] char(2) NULL,
    [s_zip] char(10) NULL,
    [s_country] varchar(20) NULL,
    [s_gmt_offset] decimal(5,2) NULL,
    [s_tax_precentage] decimal(5,2) NULL
);

CREATE TABLE dbo.[store_returns] (
    [sr_returned_date_sk] int NULL,
    [sr_return_time_sk] int NULL,
    [sr_item_sk] int NOT NULL,
    [sr_customer_sk] int NULL,
    [sr_cdemo_sk] int NULL,
    [sr_hdemo_sk] int NULL,
    [sr_addr_sk] int NULL,
    [sr_store_sk] int NULL,
    [sr_reason_sk] int NULL,
    [sr_ticket_number] int NOT NULL,
    [sr_return_quantity] int NULL,
    [sr_return_amt] decimal(7,2) NULL,
    [sr_return_tax] decimal(7,2) NULL,
    [sr_return_amt_inc_tax] decimal(7,2) NULL,
    [sr_fee] decimal(7,2) NULL,
    [sr_return_ship_cost] decimal(7,2) NULL,
    [sr_refunded_cash] decimal(7,2) NULL,
    [sr_reversed_charge] decimal(7,2) NULL,
    [sr_store_credit] decimal(7,2) NULL,
    [sr_net_loss] decimal(7,2) NULL
);

CREATE TABLE dbo.[store_sales] (
    [ss_sold_date_sk] int NULL,
    [ss_sold_time_sk] int NULL,
    [ss_item_sk] int NOT NULL,
    [ss_customer_sk] int NULL,
    [ss_cdemo_sk] int NULL,
    [ss_hdemo_sk] int NULL,
    [ss_addr_sk] int NULL,
    [ss_store_sk] int NULL,
    [ss_promo_sk] int NULL,
    [ss_ticket_number] int NOT NULL,
    [ss_quantity] int NULL,
    [ss_wholesale_cost] decimal(7,2) NULL,
    [ss_list_price] decimal(7,2) NULL,
    [ss_sales_price] decimal(7,2) NULL,
    [ss_ext_discount_amt] decimal(7,2) NULL,
    [ss_ext_sales_price] decimal(7,2) NULL,
    [ss_ext_wholesale_cost] decimal(7,2) NULL,
    [ss_ext_list_price] decimal(7,2) NULL,
    [ss_ext_tax] decimal(7,2) NULL,
    [ss_coupon_amt] decimal(7,2) NULL,
    [ss_net_paid] decimal(7,2) NULL,
    [ss_net_paid_inc_tax] decimal(7,2) NULL,
    [ss_net_profit] decimal(7,2) NULL
);

CREATE TABLE dbo.[time_dim] (
    [t_time_sk] int NOT NULL,
    [t_time_id] char(16) NOT NULL,
    [t_time] int NULL,
    [t_hour] int NULL,
    [t_minute] int NULL,
    [t_second] int NULL,
    [t_am_pm] char(2) NULL,
    [t_shift] char(20) NULL,
    [t_sub_shift] char(20) NULL,
    [t_meal_time] char(20) NULL
);

CREATE TABLE dbo.[warehouse] (
    [w_warehouse_sk] int NOT NULL,
    [w_warehouse_id] char(16) NOT NULL,
    [w_warehouse_name] varchar(20) NULL,
    [w_warehouse_sq_ft] int NULL,
    [w_street_number] char(10) NULL,
    [w_street_name] varchar(60) NULL,
    [w_street_type] char(15) NULL,
    [w_suite_number] char(10) NULL,
    [w_city] varchar(60) NULL,
    [w_county] varchar(30) NULL,
    [w_state] char(2) NULL,
    [w_zip] char(10) NULL,
    [w_country] varchar(20) NULL,
    [w_gmt_offset] decimal(5,2) NULL
);

CREATE TABLE dbo.[web_page] (
    [wp_web_page_sk] int NOT NULL,
    [wp_web_page_id] char(16) NOT NULL,
    [wp_rec_start_date] date NULL,
    [wp_rec_end_date] date NULL,
    [wp_creation_date_sk] int NULL,
    [wp_access_date_sk] int NULL,
    [wp_autogen_flag] char(1) NULL,
    [wp_customer_sk] int NULL,
    [wp_url] varchar(100) NULL,
    [wp_type] char(50) NULL,
    [wp_char_count] int NULL,
    [wp_link_count] int NULL,
    [wp_image_count] int NULL,
    [wp_max_ad_count] int NULL
);

CREATE TABLE dbo.[web_returns] (
    [wr_returned_date_sk] int NULL,
    [wr_returned_time_sk] int NULL,
    [wr_item_sk] int NOT NULL,
    [wr_refunded_customer_sk] int NULL,
    [wr_refunded_cdemo_sk] int NULL,
    [wr_refunded_hdemo_sk] int NULL,
    [wr_refunded_addr_sk] int NULL,
    [wr_returning_customer_sk] int NULL,
    [wr_returning_cdemo_sk] int NULL,
    [wr_returning_hdemo_sk] int NULL,
    [wr_returning_addr_sk] int NULL,
    [wr_web_page_sk] int NULL,
    [wr_reason_sk] int NULL,
    [wr_order_number] int NOT NULL,
    [wr_return_quantity] int NULL,
    [wr_return_amt] decimal(7,2) NULL,
    [wr_return_tax] decimal(7,2) NULL,
    [wr_return_amt_inc_tax] decimal(7,2) NULL,
    [wr_fee] decimal(7,2) NULL,
    [wr_return_ship_cost] decimal(7,2) NULL,
    [wr_refunded_cash] decimal(7,2) NULL,
    [wr_reversed_charge] decimal(7,2) NULL,
    [wr_account_credit] decimal(7,2) NULL,
    [wr_net_loss] decimal(7,2) NULL
);

CREATE TABLE dbo.[web_sales] (
    [ws_sold_date_sk] int NULL,
    [ws_sold_time_sk] int NULL,
    [ws_ship_date_sk] int NULL,
    [ws_item_sk] int NOT NULL,
    [ws_bill_customer_sk] int NULL,
    [ws_bill_cdemo_sk] int NULL,
    [ws_bill_hdemo_sk] int NULL,
    [ws_bill_addr_sk] int NULL,
    [ws_ship_customer_sk] int NULL,
    [ws_ship_cdemo_sk] int NULL,
    [ws_ship_hdemo_sk] int NULL,
    [ws_ship_addr_sk] int NULL,
    [ws_web_page_sk] int NULL,
    [ws_web_site_sk] int NULL,
    [ws_ship_mode_sk] int NULL,
    [ws_warehouse_sk] int NULL,
    [ws_promo_sk] int NULL,
    [ws_order_number] int NOT NULL,
    [ws_quantity] int NULL,
    [ws_wholesale_cost] decimal(7,2) NULL,
    [ws_list_price] decimal(7,2) NULL,
    [ws_sales_price] decimal(7,2) NULL,
    [ws_ext_discount_amt] decimal(7,2) NULL,
    [ws_ext_sales_price] decimal(7,2) NULL,
    [ws_ext_wholesale_cost] decimal(7,2) NULL,
    [ws_ext_list_price] decimal(7,2) NULL,
    [ws_ext_tax] decimal(7,2) NULL,
    [ws_coupon_amt] decimal(7,2) NULL,
    [ws_ext_ship_cost] decimal(7,2) NULL,
    [ws_net_paid] decimal(7,2) NULL,
    [ws_net_paid_inc_tax] decimal(7,2) NULL,
    [ws_net_paid_inc_ship] decimal(7,2) NULL,
    [ws_net_paid_inc_ship_tax] decimal(7,2) NULL,
    [ws_net_profit] decimal(7,2) NULL
);

CREATE TABLE dbo.[web_site] (
    [web_site_sk] int NOT NULL,
    [web_site_id] char(16) NOT NULL,
    [web_rec_start_date] date NULL,
    [web_rec_end_date] date NULL,
    [web_name] varchar(50) NULL,
    [web_open_date_sk] int NULL,
    [web_close_date_sk] int NULL,
    [web_class] varchar(50) NULL,
    [web_manager] varchar(40) NULL,
    [web_mkt_id] int NULL,
    [web_mkt_class] varchar(50) NULL,
    [web_mkt_desc] varchar(100) NULL,
    [web_market_manager] varchar(40) NULL,
    [web_company_id] int NULL,
    [web_company_name] char(50) NULL,
    [web_street_number] char(10) NULL,
    [web_street_name] varchar(60) NULL,
    [web_street_type] char(15) NULL,
    [web_suite_number] char(10) NULL,
    [web_city] varchar(60) NULL,
    [web_county] varchar(30) NULL,
    [web_state] char(2) NULL,
    [web_zip] char(10) NULL,
    [web_country] varchar(20) NULL,
    [web_gmt_offset] decimal(5,2) NULL,
    [web_tax_percentage] decimal(5,2) NULL
);
