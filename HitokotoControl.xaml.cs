using System.Net.Http;
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
                1200000); // 20分钟,有需要可以调整,单位为毫秒,1000毫秒=1秒,60000毫秒=1分钟, 1200000毫秒=20分钟
                //五分钟是300000毫秒
                // by: copilot
        }

        private async Task LoadHitokotoAsync()
        {
            try
            {
                var result = await _httpClient.GetStringAsync("https://v1.hitokoto.cn/?encode=text");
                Dispatcher.Invoke(() => HitokotoText.Text = result);
            }
            catch (HttpRequestException)
            {
                Dispatcher.Invoke(() => HitokotoText.Text = "加载失败");
            }
            catch (Exception ex)
            {
                // 处理其他可能的异常，或者记录日志
                Dispatcher.Invoke(() => HitokotoText.Text = $"发生错误: {ex.Message}");
            }
        }
    }
}

// 神秘的一言API(https://hitokoto.cn/api)提供了一言的API接口，可以通过访问这个接口获取一言。