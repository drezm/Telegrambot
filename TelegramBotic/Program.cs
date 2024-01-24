using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Practice.Contracts.Car;
using Practice.Contracts.Model;
using Practice.Contracts.CarStatus;
using System.Dynamic;

namespace TelegramBotic
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var botClient = new TelegramBotClient("6797266832:AAEqlu22rOiGopEUw0coORadk8ZQmGgWVog");
            using CancellationTokenSource cts = new();
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };
            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsyns,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token);

            var me = await botClient.GetMeAsync();
            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();
            cts.Cancel();
             


        }

        static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;
            if (message.Text is not { } messageText)
                return;
            var chatId = message.Chat.Id;
            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}");
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Ты сказал: \n" + messageText,
                cancellationToken: cancellationToken);
            if (message.Text == "Проверка")
            {
                await botClient.SendTextMessageAsync(
                     chatId: chatId,
                     text: "Провека: ОК!",
                     cancellationToken: cancellationToken
                    );
            }
            else if (message.Text == "Привет")
            {
                await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Здравствуй, Андроед!!!",
                cancellationToken: cancellationToken);
            }
            else if (message.Text == "Картинка")
            {
                await botClient.SendPhotoAsync(
                chatId: chatId,
                photo: InputFile.FromUri("https://static01.nyt.com/images/2023/12/12/climate/12cli-cats/12cli-cats-videoSixteenByNine3000.jpg"),
                cancellationToken: cancellationToken);
            }
            else if (message.Text == "Видео")
            {
                await botClient.SendVideoAsync(
                chatId: chatId,
                video: InputFile.FromUri("https://rr1---sn-n02xgoxufvg3-2gb6.googlevideo.com/videoplayback?expire=1706035317&ei=FbSvZZbBMP_C6dsP-d2q8Aw&ip=212.102.39.203&id=o-AINzZPMaBjRrY9nsc1ErgklozNgNHc5k_DVPySzaRCPU&itag=22&source=youtube&requiressl=yes&xpc=EgVo2aDSNQ%3D%3D&mh=td&mm=31%2C29&mn=sn-n02xgoxufvg3-2gb6%2Csn-2gb7sn7y&ms=au%2Crdu&mv=m&mvi=1&pl=24&initcwndbps=1320000&vprv=1&svpuc=1&mime=video%2Fmp4&cnr=14&ratebypass=yes&dur=15.928&lmt=1679196336078030&mt=1706013190&fvip=2&fexp=24007246&c=ANDROID&txp=5432434&sparams=expire%2Cei%2Cip%2Cid%2Citag%2Csource%2Crequiressl%2Cxpc%2Cvprv%2Csvpuc%2Cmime%2Ccnr%2Cratebypass%2Cdur%2Clmt&sig=AJfQdSswRQIgVIP31npCY8SfwwiddV13StrmiAl_AjxzHk4tULN61-kCIQD1OE69gtpuK6zNWfW9JPtV4ljHjUqROKWU57s3AMRaYw%3D%3D&lsparams=mh%2Cmm%2Cmn%2Cms%2Cmv%2Cmvi%2Cpl%2Cinitcwndbps&lsig=AAO5W4owRQIhAPCnmk3utw9IIBbekueKTQ2kIWxlqZiqTvJbPrlltHFEAiAL_Ef-OseAJEtqZtHqYiAFzwtNNQ7QXtnT03edkZ-3Pw%3D%3D&title=woem"),
                cancellationToken: cancellationToken);
            }
            else if (message.Text == "Стикер")
            {
                await botClient.SendStickerAsync(
                chatId: chatId,
                sticker: InputFile.FromString("CAACAgIAAxkBAAI7j2Wvsuj_wn_4DvGHdvXEAnHte45TAAJwPgAC-dJQSW3kf0jSKHISNAQ"),
                cancellationToken: cancellationToken);
            }
            else if (message.Text == "Кнопка")
            {


                // Create a reply keyboard with two buttons
                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Mersedes", "Mersedes"),
                        InlineKeyboardButton.WithCallbackData("BMW", "BMW"),
                        InlineKeyboardButton.WithCallbackData("Nissan", "Nissan"),
                        InlineKeyboardButton.WithCallbackData("Показать все машины", "Показать все машины")
                    }
                 });

                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Выберите один из вариантов:",
                    replyMarkup: inlineKeyboard,
                    cancellationToken: cancellationToken);
                if (message.Text == "Mersedes" || message.Text == "BMW" || message.Text == "Nissan" || message.Text == "Показать все машины")
            {
                
                var selectedBrand = message.Text;

                HttpClient client = new HttpClient();
                var modelsResponse = await client.GetAsync("https://localhost:7065/api/Model");
                var modelsData = await modelsResponse.Content.ReadAsStringAsync();
                UpdateModalDto[] models = JsonConvert.DeserializeObject<UpdateModalDto[]>(modelsData);
                var carsResponse = await client.GetAsync("https://localhost:7065/api/Car");
                var carsData = await carsResponse.Content.ReadAsStringAsync();
                UpdateCarDto[] cars = JsonConvert.DeserializeObject<UpdateCarDto[]>(carsData);
                




                // Предполагается, что carsWithModels уже содержит объединенные данные автомобилей и моделей


                var carsWithModels = cars.Join(models,
                                    car => car.ModelId,
                                    model => model.ModelId,
                                    (car, model) => new { Car = car, Model = model })  
                                .ToArray();
                
                              
                messageText = $"Информация о {selectedBrand}:\n";

                foreach (var item in carsWithModels.Where(x => x.Model.Brand == selectedBrand))
                {
                    var carStatusResponse = await client.GetAsync($"https://localhost:7065/api/CarStatus/{item.Car.CarStatusId}");
                    var carStatusData = await carStatusResponse.Content.ReadAsStringAsync();
                    UpdateCarStatusDto carStatus = JsonConvert.DeserializeObject<UpdateCarStatusDto>(carStatusData);

                    messageText += $" ID Машины: {item.Car.CarId} \n Модель: {item.Model.Brand} {item.Model.Model1} \n Статус:  {carStatus.StatusName} \n Год выпуска: {item.Car.ManufacturingYear} \n Пробег: {item.Car.Mileage} Км \n Описание: {item.Car.Description} \n Стоимость: {item.Car.Price} Рублей \n";

                    var photoUrl = $"{item.Car.Image}";
                    if (Uri.TryCreate(photoUrl, UriKind.Absolute, out var uri))
                    {
                        var photo = InputFile.FromUri(uri);
                        await botClient.SendPhotoAsync(chatId, photo, caption: messageText);
                    }
                    else
                    {
                        // Обработка случая, когда URL некорректен
                        Console.WriteLine("Некорректный URL");
                    }
                }
 
                if (selectedBrand == "Показать все машины")
                {

                    foreach (var item in carsWithModels)
                    {
                        var carStatusResponse = await client.GetAsync($"https://localhost:7065/api/CarStatus/{item.Car.CarStatusId}");
                        var carStatusData = await carStatusResponse.Content.ReadAsStringAsync();
                        UpdateCarStatusDto carStatus = JsonConvert.DeserializeObject<UpdateCarStatusDto>(carStatusData);

                        // Отправляем фотографию с описанием, включая цену
                        var photoUrl = $"{item.Car.Image}";
                        if (Uri.TryCreate(photoUrl, UriKind.Absolute, out var uri))
                        {
                            var photo = InputFile.FromUri(uri);
                            var caption = $" ID Машины: {item.Car.CarId} \n Модель: {item.Model.Brand} {item.Model.Model1} \n Статус: {carStatus.StatusName} \n Год выпуска: {item.Car.ManufacturingYear} \n Пробег: {item.Car.Mileage} Км \n Цена: {item.Car.Price} Рублей\n Описание: {item.Car.Description} \n ";
                            await botClient.SendPhotoAsync(chatId, photo, caption: caption);
                        }
                    }
                }
 
            }

        }
        static Task HandlePollingErrorAsyns(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException 
                => $"Telegram API Error: \n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}", 
             _ => exception.ToString() 
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;  
        }
    }
}