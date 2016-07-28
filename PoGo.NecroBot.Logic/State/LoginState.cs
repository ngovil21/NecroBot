﻿#region using directives

using System;
using System.Threading.Tasks;
using PoGo.NecroBot.Logic.Event;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;

#endregion

namespace PoGo.NecroBot.Logic.State
{
    public class LoginState : IState
    {
        public async Task<IState> Execute(ISession session)
        {
            session.EventDispatcher.Send(new NoticeEvent { Message = session.Translation.GetTranslation(Common.TranslationString.LoggingIn, session.Settings.AuthType) });
            try
            {
                switch (session.Settings.AuthType)
                {
                    case AuthType.Ptc:
                        try
                        {
                            await session.Client.Login.DoPtcLogin(session.Settings.PtcUsername, session.Settings.PtcPassword);
                        }
                        catch (AggregateException ae)
                        {
                            throw ae.Flatten().InnerException;
                        }
                        break;
                    case AuthType.Google:
                        await session.Client.Login.DoGoogleLogin(session.Settings.GoogleUsername, session.Settings.GooglePassword);
                        break;
                    default:
                        session.EventDispatcher.Send(new ErrorEvent {Message = session.Translation.GetTranslation(Common.TranslationString.WrongAuthType)});
                        return null;
                }
            }
            catch (PtcOfflineException)
            {
                session.EventDispatcher.Send(new ErrorEvent
                {
                    Message = session.Translation.GetTranslation(Common.TranslationString.PtcOffline)
                });
                session.EventDispatcher.Send(new NoticeEvent {Message = session.Translation.GetTranslation(Common.TranslationString.TryingAgainIn, 20)});
                await Task.Delay(20000);
                return this;
            }
            catch (AccountNotVerifiedException)
            {
                session.EventDispatcher.Send(new ErrorEvent {Message = session.Translation.GetTranslation(Common.TranslationString.AccountNotVerified)});
                return null;
            }

            await DownloadProfile(session);

            return new PositionCheckState();
        }

        public async Task DownloadProfile(ISession session)
        {
            session.Profile = await session.Client.Player.GetPlayer();
            session.EventDispatcher.Send(new ProfileEvent {Profile = session.Profile});
        }
    }
}