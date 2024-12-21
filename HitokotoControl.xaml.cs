using System.Net.Http;
using System.Threading.Tasks;
using ClassIsland.Core.Controls;
using System.Windows;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using MaterialDesignThemes.Wpf;

namespace HitokotoComponent
{
    [ComponentInfo(
            "3DFE17BA-92D3-9E14-F5CA-CC9EE40C5F58",
            "一言",
            PackIconKind.CalendarOutline,
            "在主界面显示一言。"
        )]
    public partial class HitokotoControl : ComponentBase
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public HitokotoControl()
        {
            InitializeComponent();
            LoadHitokotoAsync();
        }
// 新建一个定时器，用于每20分钟执行一次LoadHitokotoAsync()函数

// 创建一个定时器
System.Threading.Timer timer = new System.Threading.Timer(
    // 定时器触发时执行的回调函数
    o => LoadHitokotoAsync(),
    // 定时器触发时传递给回调函数的状态对象，这里不需要，所以传递null
    null,
    // 定时器首次触发前的延迟时间，这里设置为0，即立即执行
    0,
    // 定时器触发的间隔时间，这里设置为20分钟，即1200000毫秒
    1200000); 

// 这段代码创建了一个定时器，它会在程序启动时立即执行一次 `LoadHitokotoAsync()` 方法，并且之后每隔20分钟执行一次。
        private async void LoadHitokotoAsync()
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
        }
    }
}