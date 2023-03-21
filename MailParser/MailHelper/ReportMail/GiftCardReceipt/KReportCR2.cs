﻿using MailParser;
using Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailHelper
{
    public class KReportCR2 : KReportCR
    {
        public KReportCR2(int mail_seq_num) : base()
        {
            if (mail_seq_num == 0)
                m_mail_type = MailType.CR_2;
            else if (mail_seq_num == 1)
                m_mail_type = MailType.CR_2_1;
            else if (mail_seq_num == 2)
                m_mail_type = MailType.CR_2_2;
            else
                throw new Exception($"Invalid CR3 mail seq num. {mail_seq_num}");
        }
        public KReportCR2(MailType mail_type) : base()
        {
            m_mail_type = mail_type;
        }
        public override string make_json_text()
        {
            JObject jsonObject = null;
            string mail_address = Program.g_user.get_mailaddress_by_account_id(m_mail_account_id);
            if (m_mail_type == MailType.CR_2)
            {
                int[] child_ids = Program.g_db.get_child_report_ids(m_report_id);
                jsonObject =
                    new JObject(
                            new JProperty("mail_address", mail_address),
                            new JProperty("mail_time", m_mail_sent_date.ToString("yyyy-MM-dd HH:mm:ss")),
                            new JProperty("m_mail_ids", child_ids),
                            new JProperty("m_report_id", m_report_id),
                            new JProperty("m_mail_type", m_mail_type.ToString()),
                            new JProperty("m_order_id", m_order_id),
                            new JProperty("m_purchase_date", m_purchase_date),
                            new JProperty("m_total", m_total),
                            new JProperty("m_discount", m_discount),
                            new JProperty("m_giftcard_details", JToken.FromObject(m_giftcard_details)),
                            new JProperty("status", ConstEnv.REPORT_STATUS_ALL_RECEIVED)
                );
            }
            else if (m_mail_type == MailType.CR_2_1)
            {
                jsonObject =
                    new JObject(
                            new JProperty("mail_address", mail_address),
                            new JProperty("mail_time", m_mail_sent_date.ToString("yyyy-MM-dd HH:mm:ss")),
                            new JProperty("m_mail_id", m_mail_id),
                            new JProperty("m_report_id", m_report_id),
                            new JProperty("m_mail_type", m_mail_type.ToString()),
                            new JProperty("m_order_id", m_order_id),
                            new JProperty("m_purchase_date", m_purchase_date),
                            new JProperty("m_total", m_total),
                            new JProperty("m_discount", m_discount),
                            new JProperty("m_giftcard_details", JToken.FromObject(m_giftcard_details))
                );
            }
            else if (m_mail_type == MailType.CR_2_2)
            {
                jsonObject =
                    new JObject(
                            new JProperty("mail_address", mail_address),
                            new JProperty("mail_time", m_mail_sent_date.ToString("yyyy-MM-dd HH:mm:ss")),
                            new JProperty("m_mail_id", m_mail_id),
                            new JProperty("m_report_id", m_report_id),
                            new JProperty("m_mail_type", m_mail_type.ToString()),
                            new JProperty("m_order_id", m_order_id),
                            new JProperty("m_giftcard_details", JToken.FromObject(m_giftcard_details))
                );
            }

            if (jsonObject != null)
                return JsonConvert.SerializeObject(jsonObject, Newtonsoft.Json.Formatting.Indented);

            return "";
        }
        public override void load_from_json(string json_text)
        {
            IDictionary<string, JToken> dict = Newtonsoft.Json.Linq.JObject.Parse(json_text);

            string mail_address = (string)dict["mail_address"];
            m_mail_account_id = Program.g_user.get_account_id(mail_address);

            m_mail_sent_date = DateTime.Parse((string)dict["mail_time"]);
            m_report_id = int.Parse((string)dict["m_report_id"]);

            string mail_type = (string)dict["m_mail_type"];
            m_mail_type = (MailType)Enum.Parse(typeof(MailType), mail_type, true);

            m_order_id = (string)dict["m_order_id"];

            if (m_mail_type == MailType.CR_2)
            {
                m_total = float.Parse((string)dict["m_total"]);
                m_discount = float.Parse((string)dict["m_discount"]);
                m_purchase_date = DateTime.Parse((string)dict["m_purchase_date"]);

                m_order_status = (string)dict["status"];

                var giftcard_details = dict["m_giftcard_details"].ToList();
                foreach (IDictionary<string, JToken> giftcard_detail in giftcard_details)
                {
                    ZGiftCardDetails item = new ZGiftCardDetails()
                    {
                        m_retailer = (string)giftcard_detail["m_retailer"],
                        m_gift_card = (string)giftcard_detail["m_gift_card"],
                        m_pin = (string)giftcard_detail["m_pin"],
                        m_value = float.Parse((string)giftcard_detail["m_value"]),
                        m_cost = float.Parse((string)giftcard_detail["m_cost"])
                    };
                    m_giftcard_details.Add(item);
                }
            }
            if (m_mail_type == MailType.CR_2_1)
            {
                m_mail_id = int.Parse((string)dict["m_mail_id"]);
                m_total = float.Parse((string)dict["m_total"]);
                m_discount = float.Parse((string)dict["m_discount"]);
                m_purchase_date = DateTime.Parse((string)dict["m_purchase_date"]);

                var giftcard_details = dict["m_giftcard_details"].ToList();
                foreach (IDictionary<string, JToken> giftcard_detail in giftcard_details)
                {
                    ZGiftCardDetails item = new ZGiftCardDetails()
                    {
                        m_retailer = (string)giftcard_detail["m_retailer"],
                        m_gift_card = (string)giftcard_detail["m_gift_card"],
                        m_pin = (string)giftcard_detail["m_pin"],
                        m_value = float.Parse((string)giftcard_detail["m_value"]),
                        m_cost = float.Parse((string)giftcard_detail["m_cost"])
                    };
                    m_giftcard_details.Add(item);
                }
            }
            if (m_mail_type == MailType.CR_2_2)
            {
                m_mail_id = int.Parse((string)dict["m_mail_id"]);
                var giftcard_details = dict["m_giftcard_details"].ToList();
                foreach (IDictionary<string, JToken> giftcard_detail in giftcard_details)
                {
                    ZGiftCardDetails item = new ZGiftCardDetails()
                    {
                        m_retailer = (string)giftcard_detail["m_retailer"],
                        m_gift_card = (string)giftcard_detail["m_gift_card"],
                        m_pin = (string)giftcard_detail["m_pin"],
                        m_value = float.Parse((string)giftcard_detail["m_value"]),
                        m_cost = float.Parse((string)giftcard_detail["m_cost"])
                    };
                    m_giftcard_details.Add(item);
                }
            }
        }
        public override KReportBase merge_multi_reports()
        {
            try
            {
                // Check all mails have received.

                int child_num = 2;

                DataTable dt = Program.g_db.find_matched_cr2_reports(m_order_id, m_mail_account_id);
                if (dt == null)
                    return null;
                if (dt.Rows.Count < child_num)
                    return null;

                int[] child_mail_ids = new int[child_num];
                int[] child_report_ids = new int[child_num];
                for (int i = 0; i < child_num; i++)
                {
                    child_report_ids[i] = -1;
                    child_mail_ids[i] = -1;
                }

                foreach (DataRow row in dt.Rows)
                {
                    int id = int.Parse(row["id"].ToString());
                    int mail_id = int.Parse(row["mail_id"].ToString());

                    string mail_type_str = row["mail_type"].ToString();
                    MailType mail_type = (MailType)Enum.Parse(typeof(KReportBase.MailType), mail_type_str, true);
                    if (mail_type == MailType.CR_2_1)
                    {
                        child_report_ids[0] = id;
                        child_mail_ids[0] = mail_id;
                    }
                    else if (mail_type == MailType.CR_2_2)
                    {
                        child_report_ids[1] = id;
                        child_mail_ids[1] = mail_id;
                    }
                    else
                    {
                        return null;
                    }
                }
                if (child_report_ids.Count(s => s == -1) > 0)
                    return null;

                // Create a parent card.

                KReportCR2[] child_reports = new KReportCR2[child_report_ids.Length];
                for (int i = 0; i < child_report_ids.Length; i++)
                {
                    if (child_report_ids[i] == m_report_id)
                    {
                        child_reports[i] = this;
                    }
                    else
                    {
                        child_reports[i] = new KReportCR2(i + 1);
                        child_reports[i].make_report_from_db(child_report_ids[i]);
                    }
                }

                KReportCR2 parent_report = new KReportCR2(0);

                parent_report.set_order_id(m_order_id);
                parent_report.m_mail_account_id = child_reports[0].m_mail_account_id;
                parent_report.m_purchase_date = child_reports[0].m_purchase_date;
                parent_report.set_total(child_reports[0].m_total);
                parent_report.m_giftcard_details = child_reports[0].m_giftcard_details;
                parent_report.m_discount = child_reports[0].m_discount;
                parent_report.m_mail_sent_date = child_reports[1].m_mail_sent_date;

                if (child_reports[0].m_giftcard_details.Count != child_reports[1].m_giftcard_details.Count)
                {
                    MyLogger.Info($"*** WARNING *** mail id {child_mail_ids[0]},{child_mail_ids[1]} : detail purchase info count is different between two mails. {child_reports[0].m_giftcard_details.Count} != {child_reports[1].m_giftcard_details.Count}");
                }

                for (int i = 0; i < parent_report.m_giftcard_details.Count; i++)
                {
                    ZGiftCardDetails d1 = parent_report.m_giftcard_details[i];

                    List<ZGiftCardDetails> found = child_reports[1].m_giftcard_details.Where(s => s.m_value == d1.m_value && s.m_retailer == d1.m_retailer && s.m_cost == 0).ToList();
                    if (found.Count == 0)
                    {
                        MyLogger.Info($"*** WARNING *** : Not found matched card info. : retailer = {d1.m_retailer}, value = {d1.m_value}");
                    }
                    else
                    {
                        ZGiftCardDetails d2 = found[0];

                        d1.m_gift_card = d2.m_gift_card;
                        d1.m_pin = d2.m_pin;

                        d2.m_cost = -1; // Mark up to know it is used.
                    }
                }
                foreach (ZGiftCardDetails d in child_reports[1].m_giftcard_details) // restore child 1's cost field.
                    d.m_cost = 0;

                int parent_report_id = parent_report.insert_report_to_db(-1);
                if (parent_report_id == -1)
                    return null;
                parent_report.m_report_id = parent_report_id;

                // update parent id of all children.
                for (int i = 0; i < child_report_ids.Length; i++)
                {
                    Program.g_db.update_report_parent_id(child_report_ids[i], parent_report_id);
                }

                return parent_report;

            }
            catch (Exception exception)
            {
                MyLogger.Error($"Exception Error ({System.Reflection.MethodBase.GetCurrentMethod().Name}): {exception.Message + "\n" + exception.StackTrace}");
                return null;
            }
        }
    }
}
