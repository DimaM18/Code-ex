using System;
using Client.Scripts.GoldAndZombies.Configs.OffersConfig.RewardVariant;
using Client.Scripts.Tools.Services;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Client.Scripts.GoldAndZombies.GameHelpers.Convertors
{
    public class OfferRewardConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer jser)
        {
            JObject jsonObject = JObject.Load(reader);
            JToken rewardTypeValue = jsonObject["RewardType"];
            
            if (rewardTypeValue is null)
            {
                return null;
            }
            
            var rewardType = (OfferRewardType)rewardTypeValue.Value<int>();
            
            BaseReward reward;
            switch (rewardType)
            {
                case OfferRewardType.Chest:
                    reward = new ChestOfferReward();
                    break;
                case OfferRewardType.Cards:
                    reward = new CardOfferReward();
                    break;
                case OfferRewardType.Crystals:
                    reward = new CrystalsOfferReward();
                    break;
                case OfferRewardType.Hammers:
                    reward = new HammerOfferReward();
                    break;
                case OfferRewardType.NoAds:
                    reward = new NoAdsReward();
                    break;
                case OfferRewardType.HeroLevel:
                    reward = new HeroLevelOfferReward();
                    break;
                case OfferRewardType.HeroExperience:
                    reward = new HeroExperienceOfferReward();
                    break;
                case OfferRewardType.SkipTickets:
                    reward = new SkipTicketsReward();
                    break;
                default:
                    GlobalService.Debug.LogError($"OfferRewardConverter: unknown reward type {rewardType}");
                    return null;
            }

            jser.Populate(jsonObject.CreateReader(), reward);
            return reward;
        }
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(BaseReward);
        }
    }
}