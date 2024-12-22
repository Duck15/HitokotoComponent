using System.Net.Http;
using System.Text.Json;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using MaterialDesignThemes.Wpf;

namespace HitokotoComponent
{
    [ComponentInfo(
            "3DFE17BA-92D3-9E14-F5CA-CC9EE40C5F58",
            "一言",
            PackIconKind.CalendarOutline,
            "在ClassIsland主界面显示一言。"
        )]
    public partial class HitokotoControl : ComponentBase
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private Timer _timer;

        public HitokotoControl()
        {
            InitializeComponent();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            _timer = new Timer(
                async o => await LoadHitokotoAsync(),
                null,
                0,
                5000);  // 1200000毫秒=20分钟,有需要可以调整,单位为毫秒,
                        // 1000毫秒=1秒,60000毫秒=1分钟, 
                        // 五分钟是300000毫秒
                        // by: copilot
        }

        // Hitokoto JSON format: on https://v1.hitokoto.cn/ directly
        // {
        //     "id": 8927,
        //     "uuid": "3cf40f7f-cbae-4cd6-ad9f-c9beaa1ede65",
        //     "hitokoto": "不气盛能叫年轻人吗？",
        //     "type": "h",
        //     "from": "征服",
        //     "from_who": "刘华强",
        //     "creator": "idad wind",
        //     "creator_uid": 13374,
        //     "reviewer": 1044,
        //     "commit_from": "web",
        //     "created_at": "1667474645",
        //     "length": 10
        // }

        // 这个函数会叫DecodeHitokoto用来替代await _httpClient.GetStringAsync("https://v1.hitokoto.cn/?encode=text");
        // 只负责获取json提取hitokoto字段，和from_who字段，然后返回一个字符串
        // 例如： 不气盛能叫年轻人吗？ ——刘华强
        // 如果超过十个字，则不显示from_who字段
        // 有时，from_who会返回null，这时候就不显示from_who字段

        // 定义一个列表用于存放自定义本地一言
        private List<string> _hitokotoList = new List<string>
        {
            // # "芜~ 太，来，来。", // 请在这里添加自定义一言
            "我们的食堂没有预制菜！！ ——某杆菌由胃生的食堂",
            "本次更新，我们修复了周六不上课的bug。",
            "Damn! ——英语小子",
            "Man! What can I say? ——曼巴out",
            "十七张牌你能秒杀我？ ——某玩家",
            "这里没有一言（x",
            "三佰颗够吗？ ——Ezfic",
            "允许自定义本地一言啦！找我投稿试试吧~",
            "本插件仅用于K2三班。"
        };
        private async Task<string> DecodeHitokotoAsync()
        {
            try
            {
                //概率为15%显示自定义一言的随机一句
                if (new Random().Next(0, 100) < 15)
                {
                    return _hitokotoList[new Random().Next(0, _hitokotoList.Count)];
                }
                var result = await _httpClient.GetStringAsync("https://v1.hitokoto.cn/");
                var json = JsonDocument.Parse(result).RootElement;
                var hitokoto = json.GetProperty("hitokoto").GetString();
                var from_who = json.GetProperty("from_who").GetString();
                if (hitokoto != null && hitokoto.Length > 10)
                {
                    return hitokoto;
                }
                else if (hitokoto != null && from_who == null)
                {
                    return hitokoto;
                }
                else
                {
                    return hitokoto + " ——" + from_who;
                }
            }
            catch (HttpRequestException)
            {
                return "加载失败呐";
            }
            catch (Exception ex)
            {
                // 处理其他可能的异常，或者记录日志
                return $"喜报: {ex.Message}";
            }
        }



        private async Task LoadHitokotoAsync()
        {

            Dispatcher.Invoke(() => HitokotoText.Text = "加载中...");
            await Task.Delay(500); // 休眠500毫秒 ,由于是异步方法,所以不会阻塞UI线程
            var result = await DecodeHitokotoAsync();
            Dispatcher.Invoke(() => HitokotoText.Text = result);

        }
    }
}

// 神秘的一言API(https://hitokoto.cn/api)提供了一言的API接口，可以通过访问这个接口获取一言。