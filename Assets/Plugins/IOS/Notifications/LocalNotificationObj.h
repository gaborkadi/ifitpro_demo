//
//  LocalNotificationObj.h
//  LocalNotificationObj
//
//  Created by Yaroslav on 3/7/16.
//  Copyright © 2016 Area730. All rights reserved.
//

#import <UIKit/UIKit.h>

//! Project version number for LocalNotificationObj.
FOUNDATION_EXPORT double LocalNotificationObjVersionNumber;

//! Project version string for LocalNotificationObj.
FOUNDATION_EXPORT const unsigned char LocalNotificationObjVersionString[];

// In this header, you should import all the public headers of your framework using statements like #import <LocalNotificationObj/PublicHeader.h>

@interface LocalNotificationObj : UIViewController


-(void) setUpLocalNotification  :(NSString*) notification_id
                                :(NSString*) title
                                :(NSString*)body
                                :(NSString*)badge_number
                                :(NSString*)custome_sound
                                :(bool) is_repeating
                                :(int)repeat_unit
                                :(long)delay;



-(void) cancelNotificationById:(NSString*) notificationId;
-(void) cancelAllNotification;

-(void) clearAllNotification;

-(float) getVersion;
-(void) showToast:(NSString*)text;
-(void) applicationIconBadgeNumber:(int) badges;
-(void) minusApplicationIconBadgeNumber:(int) badges;

-(UILocalNotification *)getNotificationById:(NSString *)notificationId;


@end


