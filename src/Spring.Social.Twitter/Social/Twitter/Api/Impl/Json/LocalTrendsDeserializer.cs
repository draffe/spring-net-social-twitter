﻿#region License

/*
 * Copyright 2002-2012 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System.Collections.Generic;

using Spring.Json;

namespace Spring.Social.Twitter.Api.Impl.Json
{
    /// <summary>
    /// JSON deserializer for local trends. 
    /// </summary>
    /// <author>Bruno Baia</author>
    class LocalTrendsDeserializer : IJsonDeserializer
    {
        private const string LOCAL_TRENDS_DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:ss'Z'";

        public object Deserialize(JsonValue value, JsonMapper mapper)
        {
            LocalTrends localTrends = new LocalTrends();
            JsonValue trendsValue = value.GetValue(0);
            localTrends.Time = JsonUtils.ToDateTime(trendsValue.GetValue<string>("created_at"), LOCAL_TRENDS_DATE_FORMAT);
            foreach (JsonValue itemValue in trendsValue.GetValues("trends"))
            {
                localTrends.Items.Add(new Trend()
                {
                    Name = itemValue.GetValue<string>("name"),
                    Query = itemValue.GetValue<string>("query")
                });
            }
            return localTrends;
        }
    }
}