# Climbing Weather App (caniclimbtoday)

## DISCLAIMER
This application is not perfect nor can it be completely trusted, please use your own judgement at the crag to ensure you do not damage the rock and are safe.

## How to calculate ability to climb?
The most important aspect to whether you can climb outdoors is rock type - the type of rock dictates how all other variables impact climbing conditions. Therefore, I will be maintaining a json file that will initially cover the most common rock types found at British crags.

### Other Variables that impact climbing conditions
It is unwise to climb on rock when it is wet, it not only impacts safety but can lead to breaking the route - _we must avoid this_. I will be taking the average rainfall over the last 3 days as a good indication for how wet a crag may be, we can calculate this using historical data (and future data for predictions) and cross reference this with the rock type to see if a rock is too wet to climb. At first I am going to keep this black and white - you can or can't climb - however I hope to create a sliding scale in the future including wind speeds, sun levels, humidity, etc.

### Formula
I have adopted a formula that uses 3 measurements: rain over 24 hours, rain over 72 hours and rain over 7 days. It gives the highest weight to rain over the last 24 hours, then 72 hours, then 7 days. This is to incorporate the possibility or rocks drying over time. We will then divide this rain measurement by a rock porosity factor (RPF), meaning a lower RPF will imply the rock is more sensitive and porous.
So the formula looks like: Rain Score = (w_24 * pr_24 + w_72 * pr_72 + w_168 * pr_168)/RPF. Then this rain score can be used to infer whether it is safe to climb.