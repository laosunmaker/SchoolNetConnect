using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; // 使用 JArray 和 JObject
namespace SchoolNetConnect
{
    public class WebData
    { 
        public static async Task<JObject> GetDataLogin(string UserName,string IspType, string PassWord, string MyIp,string Driver)//联网
        {
            using var client = new HttpClient();
            if (Driver=="PC")
            {
                Driver = "0";
            }
            else if (Driver=="Mobile")
            {
                Driver = "1";
            }
            string ApiUrl =string.Format("http://10.0.9.35:801/eportal/?c=Portal&a=login&callback=&login_method=1&user_account=%2C{0}%2C{1}@{2}&user_password={3}&wlan_user_ip={4}&wlan_user_ipv6=&wlan_user_mac=000000000000&wlan_ac_ip=&wlan_ac_name=&jsVersion=3.3.3&v=10364",Driver, UserName,IspType,PassWord,MyIp);
            try
            {
                HttpResponseMessage response = await client.GetAsync(ApiUrl);
                response.EnsureSuccessStatusCode();
                string ResponseBody = await response.Content.ReadAsStringAsync();
                ResponseBody = ResponseBody.Substring(0, ResponseBody.Length - 1);
                ResponseBody = ResponseBody.Substring(1);
                JObject ResponseBodyJson = JObject.Parse(ResponseBody);//转换成json对象
                return ResponseBodyJson;
            }
            catch (System.Exception)
            {
                JObject ResponseBodyJson = JObject.Parse("{}");//转换成json对象
                return ResponseBodyJson;
            }
            
            
            
        }
        public static async Task<JObject> GetDataLogout(string MyIp)//断网
        {
            using var client = new HttpClient();
            string ApiUrl = string.Format("http://10.0.9.35:801/eportal/?c=Portal&a=logout&callback=&login_method=1&user_account=drcom&user_password=123&ac_logout=0&register_mode=1&wlan_user_ip={0}&wlan_user_ipv6=&wlan_vlan_id=1&wlan_user_mac=000000000000&wlan_ac_ip=&wlan_ac_name=&jsVersion=3.3.3&v=4341" ,MyIp);
            try
            {
                HttpResponseMessage response = await client.GetAsync(ApiUrl);
                response.EnsureSuccessStatusCode();
                string ResponseBody = await response.Content.ReadAsStringAsync();
                ResponseBody = ResponseBody.Substring(0, ResponseBody.Length - 1);
                ResponseBody = ResponseBody.Substring(1);
                JObject ResponseBodyJson = JObject.Parse(ResponseBody);//转换成json对象
                return ResponseBodyJson;
            }
            catch (System.Exception)
            {

                JObject ResponseBodyJson = JObject.Parse("{}");//转换成json对象
                return ResponseBodyJson;
            }
            
            
        }
        public static async Task<JObject> SandSmsMassage(string MyPhone)//发送sms请求
        {
            using var client = new HttpClient();
            string ApiUrl = string.Format("http://10.0.9.35:801/eportal/?c=GetQzMsg&a=sms_code&telephone={0}&type=1", MyPhone);
            try
            {
                HttpResponseMessage response = await client.GetAsync(ApiUrl);
                response.EnsureSuccessStatusCode();
                string ResponseBody = await response.Content.ReadAsStringAsync();
                ResponseBody = ResponseBody.Substring(0, ResponseBody.Length - 1);
                ResponseBody = ResponseBody.Substring(1);
                JObject ResponseBodyJson = JObject.Parse(ResponseBody);//转换成json对象
                return ResponseBodyJson;
            }
            catch (System.Exception)
            {

                JObject ResponseBodyJson = JObject.Parse("{}");//转换成json对象
                return ResponseBodyJson;
            }


        }
    }
}
