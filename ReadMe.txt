Для работы LiqPay:
1. Зарегестрироваться здесь: liqpay.ua
2. В папке Models в файле LiqPayHelper.cs заполнить Public Key и Private Key компании,
   которые можно найти в личном кабинете на сайте liqpay.ua
3. На сайте liqpay в личном кабинете в настройках страницы оплаты поставить две галки:
   3.1. Авторедирект
   3.2. POST data

Для работы recaptcha:
1. Зарегестрироваться здесь: https://www.google.com/recaptcha
2. В папке Controllers в файле HomeController заполнить secretKey
3. В папке Views/Home в файле PinCodeForm.cshtml заполнить data-sitekey