  .NET Core 8.0
  
  利用最小api方式來建立
  
  建立可使用批次或單筆方式來執行SP

  利用CancellationTokenSource來減少等待開啟資料庫時間，原因為若無法連線資料庫時，等待的時間過長，  所以設定500毫秒若無法開啟資料庫連線就直接返回錯誤結果
  
