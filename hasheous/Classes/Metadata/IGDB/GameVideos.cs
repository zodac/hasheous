﻿using System;
using Classes;
using Classes.Metadata;
using IGDB;
using IGDB.Models;

namespace hasheous_server.Classes.Metadata.IGDB
{
	public class GamesVideos
    {
        const string fieldList = "fields checksum,game,name,video_id;";

        public GamesVideos()
        {
        }


        public static GameVideo? GetGame_Videos(long? Id)
        {
            if ((Id == 0) || (Id == null))
            {
                return null;
            }
            else
            {
                Task<GameVideo> RetVal = _GetGame_Videos(SearchUsing.id, Id);
                return RetVal.Result;
            }
        }

        public static GameVideo GetGame_Videos(string Slug)
        {
            Task<GameVideo> RetVal = _GetGame_Videos(SearchUsing.slug, Slug);
            return RetVal.Result;
        }

        private static async Task<GameVideo> _GetGame_Videos(SearchUsing searchUsing, object searchValue)
        {
            // check database first
            Storage.CacheStatus? cacheStatus = new Storage.CacheStatus();
            if (searchUsing == SearchUsing.id)
            {
                cacheStatus = Storage.GetCacheStatus(Storage.TablePrefix.IGDB, "GameVideo", (long)searchValue);
            }
            else
            {
                cacheStatus = Storage.GetCacheStatus(Storage.TablePrefix.IGDB, "GameVideo", (string)searchValue);
            }

            // set up where clause
            string WhereClause = "";
            switch (searchUsing)
            {
                case SearchUsing.id:
                    WhereClause = "where id = " + searchValue;
                    break;
                case SearchUsing.slug:
                    WhereClause = "where slug = " + searchValue;
                    break;
                default:
                    throw new Exception("Invalid search type");
            }

            GameVideo returnValue = new GameVideo();
            switch (cacheStatus)
            {
                case Storage.CacheStatus.NotPresent:
                    returnValue = await GetObjectFromServer(WhereClause);
                    Storage.NewCacheValue(Storage.TablePrefix.IGDB, returnValue);
                    break;  
                case Storage.CacheStatus.Expired:
                    try
                    {
                        returnValue = await GetObjectFromServer(WhereClause);
                        Storage.NewCacheValue(Storage.TablePrefix.IGDB, returnValue, true);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine("Metadata: " + returnValue.GetType().Name + ": An error occurred while connecting to IGDB. WhereClause: " + WhereClause + ex.ToString());
                        returnValue = Storage.GetCacheValue<GameVideo>(returnValue, Storage.TablePrefix.IGDB, "id", (long)searchValue);
                    }
                    break;
                case Storage.CacheStatus.Current:
                    returnValue = Storage.GetCacheValue<GameVideo>(returnValue, Storage.TablePrefix.IGDB, "id", (long)searchValue);
                    break;
                default:
                    throw new Exception("How did you get here?");
            }

            return returnValue;
        }

        private enum SearchUsing
        {
            id,
            slug
        }

        private static async Task<GameVideo> GetObjectFromServer(string WhereClause)
        {
            // get Game_Videos metadata
            Communications comms = new Communications();
            var results = await comms.APIComm<GameVideo>(IGDBClient.Endpoints.GameVideos, fieldList, WhereClause);
            var result = results.First();

            return result;
        }
	}
}

