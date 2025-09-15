using Microsoft.Maui.Controls;

namespace SmartLockerApp.Services;

public static class GestureService
{
    /// <summary>
    /// Adds swipe gestures to a view for mobile navigation
    /// </summary>
    public static void AddSwipeGestures(View view, Func<Task>? onSwipeLeft = null, Func<Task>? onSwipeRight = null)
    {
        var swipeGestureRecognizer = new SwipeGestureRecognizer();
        
        if (onSwipeLeft != null)
        {
            swipeGestureRecognizer.Direction = SwipeDirection.Left;
            swipeGestureRecognizer.Swiped += async (s, e) => await onSwipeLeft();
            view.GestureRecognizers.Add(swipeGestureRecognizer);
        }
        
        if (onSwipeRight != null)
        {
            var rightSwipe = new SwipeGestureRecognizer
            {
                Direction = SwipeDirection.Right
            };
            rightSwipe.Swiped += async (s, e) => await onSwipeRight();
            view.GestureRecognizers.Add(rightSwipe);
        }
    }

    /// <summary>
    /// Adds pull-to-refresh gesture to a ScrollView
    /// </summary>
    public static void AddPullToRefresh(ScrollView scrollView, Func<Task> onRefresh)
    {
        var refreshView = new RefreshView();
        refreshView.Command = new Command(async () =>
        {
            refreshView.IsRefreshing = true;
            await onRefresh();
            refreshView.IsRefreshing = false;
        });
        
        // Wrap the ScrollView content in RefreshView
        var originalContent = scrollView.Content;
        scrollView.Content = null;
        refreshView.Content = originalContent;
        
        // Replace ScrollView with RefreshView in parent
        if (scrollView.Parent is Layout parent)
        {
            var index = parent.Children.IndexOf(scrollView);
            parent.Children.RemoveAt(index);
            parent.Children.Insert(index, refreshView);
        }
    }

    /// <summary>
    /// Adds long press gesture with haptic feedback
    /// </summary>
    public static void AddLongPress(View view, Func<Task> onLongPress)
    {
        var longPressGesture = new PointerGestureRecognizer();
        longPressGesture.PointerPressed += async (s, e) =>
        {
            // Start timer for long press detection
            var timer = new Timer(async _ =>
            {
                try
                {
                    // Trigger haptic feedback
                    HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
                    await onLongPress();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Long press error: {ex.Message}");
                }
            }, null, 500, Timeout.Infinite); // 500ms for long press

            longPressGesture.PointerReleased += (s2, e2) =>
            {
                timer?.Dispose();
            };
        };
        
        view.GestureRecognizers.Add(longPressGesture);
    }

    /// <summary>
    /// Adds tap gesture with visual feedback
    /// </summary>
    public static Task AddTapWithFeedbackAsync(View view, Func<Task> onTapped)
    {
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (s, e) =>
        {
            // Visual feedback
            await AnimationService.ButtonPressAsync(view);
            
            // Haptic feedback
            try
            {
#if ANDROID || IOS
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
#endif
            }
            catch { }
            
            // Execute callback
            await onTapped();
        };
        
        view.GestureRecognizers.Add(tapGesture);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Adds pinch-to-zoom gesture
    /// </summary>
    public static void AddPinchToZoom(View view, double minScale = 0.5, double maxScale = 3.0)
    {
        var pinchGesture = new PinchGestureRecognizer();
        double startScale = 1;
        
        pinchGesture.PinchUpdated += (s, e) =>
        {
            switch (e.Status)
            {
                case GestureStatus.Started:
                    startScale = view.Scale;
                    break;
                    
                case GestureStatus.Running:
                    var newScale = startScale * e.Scale;
                    newScale = Math.Max(minScale, Math.Min(maxScale, newScale));
                    view.Scale = newScale;
                    break;
                    
                case GestureStatus.Completed:
                    // Snap back to normal if too small
                    if (view.Scale < 1.0)
                    {
                        view.ScaleTo(1.0, 250, Easing.BounceOut);
                    }
                    break;
            }
        };
        
        view.GestureRecognizers.Add(pinchGesture);
    }

    /// <summary>
    /// Adds edge swipe gesture for navigation
    /// </summary>
    public static void AddEdgeSwipe(View view, Func<Task>? onBackSwipe = null)
    {
        if (onBackSwipe == null) return;

        var swipeGesture = new SwipeGestureRecognizer
        {
            Direction = SwipeDirection.Right,
            Threshold = 100 // Minimum swipe distance
        };
        
        swipeGesture.Swiped += async (s, e) =>
        {
            await onBackSwipe();
        };
        
        view.GestureRecognizers.Add(swipeGesture);
    }
}
