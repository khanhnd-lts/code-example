<template>
  <div>
      <Chat
        v-if="currentConversation"
        :participants="currentConversation.participants"
        :myself="myself"
        :messages="currentConversation.chatMessages"
        :chat-title="currentConversation.title"
        :placeholder="placeholder"
        :colors="colors"
        :border-style="borderStyle"
        :hide-close-button="hideCloseButton"
        :close-button-icon-size="closeButtonIconSize"
        :submit-icon-size="submitIconSize"
        :submit-image-icon-size="submitImageIconSize"
        :load-more-messages="null"
        :loading-messages="loadingConversation"
        :async-mode="asyncMode"
        :scroll-bottom="scrollBottom"
        :display-header="true"
        :send-images="true"
        :profile-picture-config="profilePictureConfig"
        :timestamp-config="timestampConfig"
        @onImageClicked="onImageClicked"
        @onImageSelected="onImageSelected"
        @onMessageSubmit="onMessageSubmit"
        @onType="onType"
        @onClose="onClose"
        style="
          position: fixed;
          bottom: 0px;
          right: 260px;
          width: 320px;
          height: 400px;
        "
      >
        <template #close-button>
          <v-icon
            v-if="
              currentConversation &&
              currentConversation.participants &&
              currentConversation.participants.length > 0
            "
            class="ml-1"
            @click="voiceCall(currentConversation.participants[0].account)"
          >
            mic
          </v-icon>
          <v-icon class="ml-1" @click="onClose">close</v-icon>
        </template>
      </Chat>
      <v-navigation-drawer
        v-model="$store.state.app.chatDrawer"
        right
        width="240"
        floating
        class="chat-container"
      >
        <perfect-scrollbar
          class="chat-drawer-menu--scroll"
          :settings="scrollSettings"
        >
          <v-card class="chat-content pa-4 mt-3" flat>
            <template v-if="this.searchContact !== ''">
              <div v-if="loadingContacts" class="text-center pa-4">
                <v-progress-circular
                  indeterminate
                  color="primary"
                  size="30"
                  width="3"
                ></v-progress-circular>
              </div>
              <v-list v-else>
                <div class="chat-group-title">
                  <span>Người liên hệ</span>
                </div>
                <div v-if="this.contacts.length == 0" class="py-4 text-center">
                  Không có kết quả
                </div>
                <v-list-item
                  v-else
                  v-for="item in contacts"
                  :key="item.id"
                  @click="openSupportContact(item)"
                  class="chat-item-user"
                  :title="item.hoTen"
                >
                  <v-list-item-avatar class="mr-1 border-avatar" size="40">
                    <v-badge
                      bordered
                      bottom
                      color="success"
                      dot
                      offset-x="10"
                      offset-y="10"
                      class="mt-0 avatar-badge"
                      v-if="item.online"
                    >
                      <v-avatar color="grey lighten-1" size="40">
                        <v-img
                          :src="
                            HOST +
                            '/api/fileupload/download/' +
                            item.linkAnhCaNhan
                          "
                        ></v-img>
                      </v-avatar>
                    </v-badge>
                    <v-avatar color="grey lighten-1" size="40" v-else>
                      <v-img
                        :src="
                          HOST +
                          '/api/fileupload/download/' +
                          item.linkAnhCaNhan
                        "
                      ></v-img>
                    </v-avatar>
                  </v-list-item-avatar>
                  <v-list-item-content>
                    <v-list-item-title>
                      <span class="chat-username">
                        {{ item.hoTen }}
                      </span>
                    </v-list-item-title>
                  </v-list-item-content>
                </v-list-item>
              </v-list>
            </template>
            <template v-else>
              <v-list>
                <div class="chat-group-title">
                  <span>Trợ giảng</span>
                </div>
                <v-list-item
                  v-for="item in supporterContactsSorted"
                  :key="item.id"
                  @click="openSupportContact(item)"
                  class="chat-item-user"
                  :title="item.hoTen"
                >
                  <v-list-item-avatar class="mr-1 border-avatar" size="40">
                    <v-badge
                      bordered
                      bottom
                      color="success"
                      dot
                      offset-x="10"
                      offset-y="10"
                      class="mt-0 avatar-badge"
                      v-if="item.online"
                    >
                      <v-avatar color="grey lighten-1" size="40">
                        <v-img
                          :src="
                            HOST +
                            '/api/fileupload/download/' +
                            item.linkAnhCaNhan
                          "
                        ></v-img>
                      </v-avatar>
                    </v-badge>
                    <v-avatar color="grey lighten-1" size="40" v-else>
                      <v-img
                        :src="
                          HOST +
                          '/api/fileupload/download/' +
                          item.linkAnhCaNhan
                        "
                      ></v-img>
                    </v-avatar>
                  </v-list-item-avatar>
                  <v-list-item-content>
                    <v-list-item-title>
                      <span class="chat-username">
                        {{ item.hoTen }}
                      </span>
                    </v-list-item-title>
                  </v-list-item-content>
                </v-list-item>
              </v-list>

              <v-list class="mt-2">
                <div class="chat-group-title">
                  <span>Hội thoại gần đây</span>
                </div>
                <v-progress-circular
                  v-if="loadingConversationHistories"
                  indeterminate
                  color="primary"
                ></v-progress-circular>
                <v-list-item
                  v-else
                  v-for="item in conversationHistoriesSorted"
                  :key="item.id"
                  @click="openConversation(item)"
                  class="chat-item-user"
                  dense
                  :title="getProfileName(item)"
                >
                  <v-list-item-avatar class="mr-1 border-avatar" size="40">
                    <v-badge
                      bordered
                      bottom
                      color="success"
                      dot
                      offset-x="10"
                      offset-y="10"
                      class="mt-0 avatar-badge"
                      v-if="item.online"
                    >
                      <v-avatar color="grey lighten-1" size="40">
                        <v-img
                          :src="
                            HOST +
                            '/api/fileupload/download/' +
                            getProfilePicture(item)
                          "
                        ></v-img>
                      </v-avatar>
                    </v-badge>
                    <v-avatar color="grey lighten-1" size="40" v-else>
                      <v-img
                        :src="
                          HOST +
                          '/api/fileupload/download/' +
                          getProfilePicture(item)
                        "
                      ></v-img>
                    </v-avatar>
                  </v-list-item-avatar>
                  <v-list-item-content class="pb-1">
                    <v-list-item-title>
                      <span class="chat-username">
                        {{ getProfileName(item) }}
                      </span>
                    </v-list-item-title>
                    <v-list-item-subtitle v-if="item.lastChatMessage">
                      <span class="lastMessage">
                        {{ item.lastChatMessage.content }}
                      </span>
                    </v-list-item-subtitle>
                  </v-list-item-content>
                  <v-list-item-icon class="mt-3 ml-2">
                    <v-badge
                      color="error"
                      v-if="item.totalUnSeenChatMessages > 0"
                      :content="item.totalUnSeenChatMessages"
                      small
                      overlap
                    >
                      <v-icon :color="'primary'">chat_bubble</v-icon>
                    </v-badge>
                    <v-icon v-else :color="'grey'" small>chat_bubble</v-icon>
                  </v-list-item-icon>
                </v-list-item>
              </v-list>

              <v-list class="mt-2" v-if="usersOnline && usersOnline.length > 0">
                <div class="chat-group-title">
                  <span>Onlines ({{ usersOnline.length }})</span>
                </div>
                <v-list-item
                  v-for="item in usersOnline"
                  :key="item.id"
                  @click="openSupportContact(item)"
                  class="chat-item-user"
                  :title="item.hoTen"
                >
                  <v-list-item-avatar class="mr-1 border-avatar" size="40">
                    <v-badge
                      bordered
                      bottom
                      color="success"
                      dot
                      offset-x="10"
                      offset-y="10"
                      class="mt-0 avatar-badge"
                    >
                      <v-avatar color="grey lighten-1" size="40">
                        <v-img
                          :src="
                            HOST +
                            '/api/fileupload/download/' +
                            item.linkAnhCaNhan
                          "
                        ></v-img>
                      </v-avatar>
                    </v-badge>
                  </v-list-item-avatar>
                  <v-list-item-content>
                    <v-list-item-title>
                      <span class="chat-username">
                        {{ item.hoTen }}
                      </span>
                    </v-list-item-title>
                  </v-list-item-content>
                  <v-list-item-action class="ml-2">
                    <v-menu
                      rounded="lg"
                      bottom
                      left
                      offset-y
                      :close-on-content-click="false"
                      transition="scale-transition"
                      origin="top right"
                    >
                      <template v-slot:activator="{ on, attrs }">
                        <v-btn small icon v-bind="attrs" v-on="on">
                          <v-icon small :color="'gray'">monitor</v-icon>
                        </v-btn>
                      </template>
                      <v-list nav>
                        <v-list-item @click="showMonitor(item)">
                          <v-list-item-title>
                            <v-icon left :color="'gray'">monitor</v-icon>
                            Screen
                          </v-list-item-title>
                        </v-list-item>
                        <v-list-item @click="voiceCall(item.account)">
                          <v-list-item-title>
                            <v-icon left :color="'gray'">mic</v-icon>
                            Call
                          </v-list-item-title>
                        </v-list-item>
                      </v-list>
                    </v-menu>
                  </v-list-item-action>
                </v-list-item>
              </v-list>
            </template>
          </v-card>
        </perfect-scrollbar>
        <template v-slot:append>
          <div class="pa-2 pt-0">
            <v-text-field
              placeholder="Tìm kiếm..."
              prepend-inner-icon="search"
              solo
              flat
              dense
              v-model="searchContact"
              @input="getContact"
              class="mt-0 pt-0"
              hide-details
            ></v-text-field>
          </div>
        </template>
      </v-navigation-drawer>
    <v-dialog v-model="showScreenSharing" scrollable persistent fullscreen dark>
      <v-card tile>
        <div v-if="!connectFailed" class="user-share-screen-info__wrapper">
          <div class="user-share-screen-info__content">
            Bạn đang xem màn hình của {{ userScreenSelected.hoTen }}
          </div>
        </div>
        <div class="button-wrapper">
          <v-menu
            v-if="usersShareScreen && usersShareScreen.length > 0"
            rounded="lg"
            bottom
            left
            offset-y
            :close-on-content-click="false"
            transition="slide-y-transition"
          >
            <template v-slot:activator="{ on }">
              <v-btn
                fab
                height="40"
                width="40"
                depressed
                color="#e4e6eb"
                v-on="on"
              >
                <v-icon color="grey darken-1">monitor</v-icon>
              </v-btn>
            </template>
            <v-list nav>
              <div class="my-2">Người dùng đang chia sẻ</div>
              <v-list-item
                class="mb-0"
                v-for="(userShare, iUserShare) in usersShareScreen"
                :key="iUserShare"
                @click="showMonitor(userShare)"
              >
                <v-list-item-avatar size="36" class="mr-2 my-2">
                  <v-img
                    :src="
                      userShare.linkAnhCaNhan
                        ? HOST +
                          '/api/fileupload/download/' +
                          userShare.linkAnhCaNhan
                        : '/static/img/default-avatar.png'
                    "
                    class="grey lighten-2 border-img--1"
                  ></v-img>
                </v-list-item-avatar>
                <v-list-item-content>
                  <div class="mr-1" style="font-size: 16px; color: #050505">
                    {{ userShare.hoTen ? userShare.hoTen : "Hviter" }}
                  </div>
                </v-list-item-content>
              </v-list-item>
            </v-list>
          </v-menu>
          <v-btn
            fab
            height="40"
            width="40"
            depressed
            color="#e4e6eb"
            class="ml-4"
            ref="btnCall"
            @click="voiceCall(connection.otherScreenSharingPeers[0], true)"
          >
            <v-icon color="grey darken-1">mdi-phone</v-icon>
          </v-btn>
          <v-btn
            fab
            height="40"
            width="40"
            depressed
            color="#e4e6eb"
            class="ml-4"
            @click="closeShareScreen"
          >
            <v-icon color="grey darken-1">mdi-close</v-icon>
          </v-btn>
        </div>
        <v-card-text class="pa-0 d-flex justify-center align-center">
          <div
            v-show="!connectFailed"
            id="connect-wrapper"
            class="text-center"
          ></div>
          <div v-show="connectFailed" class="connect-error text-center">
            <div class="mb-4">Đã xảy ra lỗi. Vui lòng thử lại</div>
            <v-btn outlined @click="showMonitor(userScreenSelected)">
              Thử lại
            </v-btn>
          </div>
        </v-card-text>
      </v-card>
    </v-dialog>
    <v-snackbar v-model="showVoiceCalling" :timeout="-1">
      <div>
        {{ isCaller ? voiceCallingModel.to : voiceCallingModel.from }}
        <template v-if="isOutcomingCall && !accepted"> đang gọi...</template>
        <template v-if="accepted"> đang trong cuộc gọi</template>
      </div>

      <template v-slot:action="{}">
        <v-btn
          @click="acceptIncomingCall"
          v-if="!isOutcomingCall && !accepted"
          height="36"
          width="36"
          fab
          dark
          color="success"
          depressed
          class="mx-1 my-2"
        >
          <v-icon dark>call</v-icon>
        </v-btn>
        <v-btn
          v-if="!accepted"
          @click="rejectIncomingCall"
          fab
          dark
          height="36"
          width="36"
          color="error"
          depressed
          class="mx-1 my-2"
        >
          <v-icon dark>call_end</v-icon>
        </v-btn>
        <v-btn
          v-if="
            accepted && (!voiceCallingModel.autoAcceptCall || showScreenSharing)
          "
          @click="stopCalling"
          fab
          dark
          height="36"
          width="36"
          color="error"
          depressed
          class="mx-1 my-2"
        >
          <v-icon dark>call_end</v-icon>
        </v-btn>
        <v-btn
          @click="disableSpeaker"
          v-if="accepted"
          height="36"
          width="36"
          fab
          dark
          color="error"
          depressed
          class="mx-1 my-2"
        >
          <v-icon dark>{{
            isDisableSpeaker ? "mdi-volume-mute" : "mdi-volume-high"
          }}</v-icon>
        </v-btn>
      </template>
    </v-snackbar>
    <v-snackbar
      v-model="pushNotificationShare"
      :timeout="5000"
      left
      transition="slide-y-reverse-transition"
    >
      {{ pushNotificationText }}
      <template v-slot:action="{ attrs }">
        <v-icon
          dark
          v-bind="attrs"
          @click="pushNotificationShare = false"
          color="#ffffffde"
        >
          mdi-close-circle-outline
        </v-icon>
      </template>
    </v-snackbar>
  </div>
</template>

<script lang="ts">
import { Vue } from "vue-property-decorator";
import * as signalR from "@microsoft/signalr";
import Chat from "@/components/Commons/Chat/Chat.vue";
import HocVienApi from "../../apiResources/HocVienApi";
import FileUploadApi from "../../apiResources/FileUploadApi";
import { HocVienDTO } from "@/models/HocVienDTO";
const Peer = require("simple-peer");

export default Vue.extend({
  components: { Chat },
  data() {
    return {
      isCaller: false,
      autoAccept: false,
      userWatchingMyScreen: null,
      pushNotificationShare: false,
      pushNotificationText: "",
      showCall: true,
      incomingCallAudio: null as any,
      busySignal: null as any,
      waitingSignal: null as any,
      remoteAudio: null as any,
      voiceCallingModel: {
        from: "",
        to: "",
        autoAcceptCall: false,
      },
      isConfirmed: false,
      isCalling: false,
      isOutcomingCall: false,
      accepted: false,
      isMuted: false,
      isDisableSpeaker: false,
      streamVoiceCalling: undefined as any,
      //
      showVoiceCalling: false,
      showScreenSharing: false,
      stream: undefined,
      //
      scrollSettings: {
        maxScrollbarLength: 160,
      },
      connecting: false,
      dialog: false,
      HOST: process.env.VUE_APP_ROOT_API,
      connection: {} as any,
      supporterContacts: [] as any[],
      conversationHistories: [] as any[],
      fab: false,
      // chat
      visible: true,
      participants: [],
      myself: {
        account: "",
        name: "",
        id: 0,
        profilePicture: "",
      },
      messages: [] as any[],
      chatTitle: "",
      placeholder: "",
      colors: {
        header: {
          bg: "#ffffff",
          text: "#050505",
        },
        message: {
          myself: {
            bg: "#0084FF",
            text: "#fff",
          },
          others: {
            bg: "#E4E6EB",
            text: "#050505",
          },
          messagesDisplay: {
            bg: "#fff",
          },
        },
        submitIcon: "#0084FF",
        submitImageIcon: "#0084FF",
      },
      borderStyle: {
        topLeft: this.$vuetify.breakpoint.smAndUp ? "10px" : "0",
        topRight: this.$vuetify.breakpoint.smAndUp ? "10px" : "0",
        bottomLeft: "0",
        bottomRight: "0",
      },
      hideCloseButton: false,
      submitIconSize: 22,
      submitImageIconSize: 22,
      closeButtonIconSize: "20px",
      asyncMode: true,
      toLoad: [],
      scrollBottom: {
        messageSent: true,
        messageReceived: false,
      },
      profilePictureConfig: {
        others: true,
        myself: true,
        styles: {
          width: "30px",
          height: "30px",
          borderRadius: "50%",
        },
      },
      timestampConfig: {
        format: "HH:mm D [thg] M, YYYY",
        relative: false,
      },
      conversations: [] as any[],
      currentConversation: null as any,
      totalUnSeenChatMessages: 0 as number,
      loadingConversationHistories: true,
      loadingHub: true,
      loadingConversation: false,
      loadingContacts: false,
      searchContact: "" as any,
      contacts: [] as any[],
      onlines: [] as any[],
      usersOnline: [] as any[],
      usersShareScreen: [] as any[],
      userScreenSelected: {} as HocVienDTO,
      connectFailed: false,
    };
  },
  computed: {
    supporterContactsSorted(): any[] {
      return (
        this.supporterContacts
          //.filter((item: any) => {
          //    if (this.searchContact !== '')
          //        return (item.hoTen.indexOf(this.searchContact) > -1 || item.account.indexOf(this.searchContact) > -1);
          //    return true;
          //})
          .sort((a: any, b: any): any => {
            if (a.online) return -1;
            if (b.online) return 1;
            return 0;
          })
      );
    },
    conversationHistoriesSorted(): any[] {
      return (
        this.conversationHistories
          //.filter((item: any) => {
          //    if (this.searchContact !== '')
          //        return (item.name.indexOf(this.searchContact) > -1);
          //    return true;
          //})
          .sort((a: any, b: any): any => {
            if (a.totalUnSeenChatMessages > b.totalUnSeenChatMessages)
              return -1;
            if (a.totalUnSeenChatMessages < b.totalUnSeenChatMessages) return 1;
            if (a.online) return -1;
            if (b.online) return 1;
            return 0;
          })
      );
    },
  },
  watch: {
    totalUnSeenChatMessages: function (newVal) {
      this.$emit("pushTotalUnSeenChatMessages", newVal);
    },
    usersOnline(newVal) {
      this.$store.commit("GET_USERONLINES", newVal);
    },
    usersShareScreen(newVal) {
      this.$store.commit("UPDATE_USERSSHARESCREEN", newVal);
    },
    "myself.account"(newVal) {
      this.connection.identifier = newVal;
    },
  },
  created() {
    this.incomingCallAudio = new Audio("/static/phone_ringtone.mp3");
    this.remoteAudio = new Audio();
    this.waitingSignal = new Audio("/static/waiting-signal.mp3");
    this.busySignal = new Audio("/static/busy-signal.mp3");
    this.incomingCallAudio.loop = true;
    this.waitingSignal.loop = true;
    this.remoteAudio.autoplay = true;

    this.loadingHub = true;
    this.getMyProfile();
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(process.env.VUE_APP_ROOT_API + "/chathub")
      .withAutomaticReconnect()
      .build();
    this.connection.onclose(async () => {
      await this.start();
    });
    this.start();

    this.connection.on("PushUserOnline", (res: any) => {
      this.onlines.push(res);
      for (let i = 0; i < this.conversationHistories.length; i++) {
        if (
          this.conversationHistories[i].participants.length == 1 &&
          this.conversationHistories[i].participants[0].account == res
        )
          Vue.set(this.conversationHistories[i], "online", true);
        else {
          for (
            let j = 0;
            j < this.conversationHistories[i].participants.length;
            j++
          ) {
            if (
              this.conversationHistories[i].participants[j].account == res &&
              res !== this.myself.account
            ) {
              Vue.set(this.conversationHistories[i], "online", true);
            }
          }
        }
      }
      for (let i = 0; i < this.supporterContacts.length; i++) {
        if (this.supporterContacts[i].account == res) {
          Vue.set(this.supporterContacts[i], "online", true);
          break;
        }
      }
    });
    this.connection.on("PushUsersOnline", (res: any) => {
      this.onlines = this.onlines.concat(res);
      for (let i = 0; i < this.conversationHistories.length; i++) {
        if (
          this.conversationHistories[i].participants.length == 1 &&
          this.onlines.indexOf(
            this.conversationHistories[i].participants[0].account
          ) > -1
        )
          Vue.set(this.conversationHistories[i], "online", true);
        else {
          for (
            let j = 0;
            j < this.conversationHistories[i].participants.length;
            j++
          ) {
            if (
              this.onlines.indexOf(
                this.conversationHistories[i].participants[j].account
              ) > -1 &&
              this.conversationHistories[i].participants[j].account !==
                this.myself.account
            ) {
              Vue.set(this.conversationHistories[i], "online", true);
            }
          }
        }
      }
      for (let i = 0; i < this.supporterContacts.length; i++) {
        if (this.onlines.indexOf(this.supporterContacts[i].account) > -1) {
          Vue.set(this.supporterContacts[i], "online", true);
          break;
        }
      }
    });
    this.connection.on("PushUsersOnlineAdmin", (res: any) => {
      this.usersOnline = res;
    });
    this.connection.on("PushUsersShareScreen", (res: any) => {
      this.usersShareScreen = res;
    });
    this.connection.on("OnStartShareScreen", (res: any) => {
      this.usersShareScreen.push(res);
      this.handleOnUserShare(`${res.hoTen} đã chia sẻ màn hình`);
    });
    this.connection.on("OnStopShareScreen", (res: any) => {
      let idx = this.usersShareScreen.findIndex((x) => x.id === res.id);
      if (idx !== -1) {
        this.usersShareScreen.splice(idx, 1);
        this.handleOnUserShare(`${res.hoTen} đã dừng chia sẻ màn hình`);
      }
    });
    this.connection.on("PushUserOffline", (res: any) => {
      let idx = this.onlines.indexOf(res);
      this.onlines.splice(idx, 1);
      for (let i = 0; i < this.conversationHistories.length; i++) {
        if (
          this.conversationHistories[i].participants.length == 1 &&
          this.conversationHistories[i].participants[0].account == res
        )
          Vue.set(this.conversationHistories[i], "online", false);
        else {
          for (
            let j = 0;
            j < this.conversationHistories[i].participants.length;
            j++
          ) {
            if (
              this.conversationHistories[i].participants[j].account == res &&
              res !== this.myself.account
            ) {
              Vue.set(this.conversationHistories[i], "online", false);
            }
          }
        }
      }
      for (let i = 0; i < this.supporterContacts.length; i++) {
        if (this.supporterContacts[i].account == res) {
          Vue.set(this.supporterContacts[i], "online", false);
          break;
        }
      }
    });
    this.connection.on("PushConversations", (res: any) => {
      this.loadingConversationHistories = false;
      //console.log(res);
      this.conversationHistories = res.data;
    });

    this.connection.on("PushSupporterContacts", (res: any) => {
      //console.log(res);
      this.supporterContacts = res.data;
    });

    this.connection.on("PushContacts", (res: any) => {
      let contacts = [];

      for (let i = 0; i < res.data.length; i++) {
        if (this.onlines.indexOf(res.data[i].account) > -1) {
          res.data[i].online = true;
        }
        contacts.push(res.data[i]);
      }
      this.contacts = contacts;
      this.loadingContacts = false;
    });

    this.connection.on("PushAddOrOpenConversation", (res: any) => {
      if (!res.messages) res.messages = [];
      this.currentConversation = res;
      this.loadingConversation = false;
      //this.conversations.push(res.data);
    });

    this.connection.on("PushNewChatMessage", (res: any) => {
      //console.log(res);
      if (
        (this.currentConversation &&
          this.currentConversation != null &&
          this.currentConversation.id == res.conversationId) ||
        this.loadingConversation
      ) {
        this.currentConversation.chatMessages.push(res);
        this.connection
          .invoke("MarkSeenChatMessage", res.id)
          .catch((err: any) => {
            console.error(err);
          });
      } else {
        this.playSoundNewMessage();
        this.totalUnSeenChatMessages += 1;
        let conversationExits = false;
        for (let i = 0; i < this.conversationHistories.length; i++) {
          if (this.conversationHistories[i].id == res.conversationId) {
            this.conversationHistories[i].totalUnSeenChatMessages += 1;
            conversationExits = true;
            break;
          }
        }
        if (!conversationExits) {
          this.conversationHistories.push({
            online: true,
            participants: [this.myself, res.creator],
            totalUnSeenChatMessages: 1,
            id: res.conversationId,
          });
        }
        this.$emit("pushTotalUnSeenChatMessages", this.totalUnSeenChatMessages);
        //this.openConversation({ id: res.conversationId })
      }
    });

    this.connection.on("PushTotalUnSeenChatMessages", (res: any) => {
      // console.log(res);
      this.totalUnSeenChatMessages = res;
      this.$emit("pushTotalUnSeenChatMessages", this.totalUnSeenChatMessages);
    });
    this.connection.on("StartScreenMonitoring", this.onStartScreenMonitoring);
    this.connection.on("OnWatchScreenShare", (res: any) => {
      this.userWatchingMyScreen = res;
      this.userScreenSelected.account = res;
      if (!this.$store.state.app.sharingScreen)
        this.handleOnUserShare(
          `${this.userWatchingMyScreen} đã yêu cầu bạn chia sẻ màn hình`
        );
      else
        this.handleOnUserShare(
          `${this.userWatchingMyScreen} đang xem màn hình của bạn`
        );
    });
    this.connection.on("OnRequestDisconnection", this.onRequestDisconnection);
    this.connection.on("OnResponseDisconnection", this.onResponseDisconnection);
    this.connection.on(
      "OnRequestOtherDisconnection",
      this.onRequestOtherDisconnection
    );
    this.connection.on("VoiceCalling", this.onVoiceCalling);
    this.connection.on("VoiceCallingAccepted", this.onVoiceCallingAccepted);
    this.connection.on("VoiceCallingRejected", this.onVoiceCallingRejected);
    this.connection.on("VoiceCallingStoped", this.onVoiceCallingStoped);
  },
  mounted() {
    this.$eventBus.$on("showMonitorExternal", this.showMonitor);
    this.$eventBus.$on("voiceCallExternal", this.voiceCall);
    this.$eventBus.$on("openChatExternal", this.openSupportContact);
  },
  methods: {
    async start() {
      this.loadingHub = true;
      try {
        this.connection.identifier = this.myself.account;
        this.connection.roomId = "roomId";
        this.connection.otherScreenSharingPeers = []; //Lưu những user khác connect
        this.connection.otherVoiceCallingPeers = []; //Lưu những user khác connect

        await this.connection.start();

        this.loadingHub = false;
      } catch (err) {
        console.log("start", err);
        setTimeout(() => this.start(), 3000);
      }
    },
    getMyProfile() {
      HocVienApi.getThongTinHocVien()
        .then((res) => {
          this.myself = {
            account: res.account,
            name: res.hoTen,
            id: res.id,
            profilePicture: res.linkAnhCaNhan,
          };
        })
        .catch((res) => {
          this.$snotify.error("Lấy thông tin thất bại!");
        });
    },
    showChatContacts() {
      let app = this.$store.state.app;
      app.chatDrawer = !this.$store.state.app.chatDrawer;
      this.$store.commit("UPDATE_APP", app);
    },
    openConversation(conversation: any) {
      this.loadingConversation = true;
      conversation.chatMessages = [];
      this.currentConversation = conversation;
      //console.log(this.currentConversation);
      this.totalUnSeenChatMessages -= conversation.totalUnSeenChatMessages;
      conversation.totalUnSeenChatMessages = 0;
      this.connection
        .invoke("AddOrOpenConversation", conversation)
        .catch((err: any) => {
          console.error(err);
        });
    },
    openSupportContact(contact: any) {
      this.loadingConversation = true;
      let conversation = {
        title: contact.hoTen,
        participants: [
          {
            account: contact.account,
            isCreator: false,
          },
        ],
        chatMessages: [],
      };
      this.currentConversation = conversation;
      this.connection
        .invoke("AddOrOpenConversation", conversation)
        .catch((err: any) => {
          console.error(err);
        });
    },
    //chat
    // eslint-disable-next-line
    onType: function (e: any) {
      // eslint-disable-next-line
      //console.log('typing');
    },
    loadMoreMessages(resolve: any) {
      setTimeout(() => {
        resolve(this.toLoad);
        //Make sure the loaded messages are also added to our local messages copy or they will be lost
        this.messages.unshift(...this.toLoad);
        this.toLoad = [];
      }, 500);
    },
    onMessageSubmit(message: any, pushMessage = true, index = -1) {
      message.loading = true;
      message.viewed = false;
      if (pushMessage) {
        index = this.currentConversation.chatMessages.length;
        message.creator = {
          conversationId: this.currentConversation.id,
          account: this.myself.account,
          name: this.myself.name,
          profilePicture: this.myself.profilePicture,
        };
        this.currentConversation.chatMessages.push(message);
      }

      this.connection
        .invoke(
          "SendMessage",
          this.currentConversation.id,
          message.content,
          message.photo,
          message.file
        )
        .then((res: any) => {
          message = res;
          message.loading = false;
          message.viewed = true;
          if (index != -1) {
            Vue.set(this.currentConversation.chatMessages, index, message);
          }
        })
        .catch((err: any) => {
          console.error(err);
        });
    },
    onClose(param: any) {
      this.currentConversation = null;
    },
    onImageSelected({ file, message }: any) {
      message.creator = {
        conversationId: this.currentConversation.id,
        account: this.myself.account,
        name: this.myself.name,
        profilePicture: this.myself.profilePicture,
      };
      let index = this.currentConversation.chatMessages.length;
      this.currentConversation.chatMessages.push(message);
      // upload image
      var formData = new FormData();
      let inpFile = document.querySelector("#inputImage") as any;
      formData.append("img", inpFile.files[0]);
      inpFile.value = null;
      FileUploadApi.upload(formData)
        .then((res) => {
          message.photo = res.key;
          this.onMessageSubmit(message, false, index);
          //this.$snotify.success("Upload ảnh thành công!");
        })
        .catch((res) => {
          this.$snotify.error("Upload ảnh thất bại!");
        });
    },
    onImageClicked(message: any) {
      /**
       * This is the callback function that is going to be executed when some image is clicked.
       * You can add your code here to do whatever you need with the image clicked. A common situation is to display the image clicked in full screen.
       */
      // console.log('Image clicked', message.src)
    },
    sendMessage() {},
    show() {
      this.dialog = true;
    },
    hide() {
      this.dialog = false;
    },
    onBackButtonClicked() {
      if (this.currentConversation !== null) {
        this.currentConversation = null;
      } else {
        this.hide();
      }
    },
    getProfilePicture(item: any) {
      if (item.participants.length == 1) {
        return item.participants[0].profilePicture;
      } else if (item.participants.length == 2) {
        if (item.participants[0].account == this.myself.account) {
          return item.participants[1].profilePicture;
        } else {
          return item.participants[0].profilePicture;
        }
      } else {
        item.participants
          .map(function (elem: any) {
            return elem.profilePicture;
          })
          .join(", ");
      }
    },
    getProfileName(item: any) {
      if (item.participants.length == 1) {
        return item.participants[0].name;
      } else if (item.participants.length == 2) {
        if (item.participants[0].account == this.myself.account) {
          return item.participants[1].name;
        } else {
          return item.participants[0].name;
        }
      } else {
        item.participants
          .map(function (elem: any) {
            return elem.name;
          })
          .join(", ");
      }
    },
    getContact(val: any) {
      this.loadingContacts = true;
      this.connection.invoke("GetContacts", val).catch((err: any) => {
        console.error(err);
      });
    },

    playSoundNewMessage() {
      var audio = new Audio("/static/new-message.mp3");
      audio.play();
    },
    //
    // screen monitoring
    closeShareScreen() {
      this.showScreenSharing = false;
      this.destroyMyPeer();
    },
    handleOnUserShare(text: any) {
      this.pushNotificationShare = true;
      this.pushNotificationText = text;
    },
    disableMic() {},
    disableSpeaker() {
      if (this.remoteAudio.paused) {
        this.remoteAudio.play();
        this.isDisableSpeaker = false;
      } else {
        this.remoteAudio.pause();
        this.isDisableSpeaker = true;
      }
    },
    updateScreenShare(value: boolean) {
      let app = this.$store.state.app;
      app.sharingScreen = value;
      this.$store.commit("UPDATE_APP", app);
    },
    async acceptIncomingCall() {
      console.log("acceptIncomingCall");
      this.accepted = true;
      this.incomingCallAudio.pause();
      this.connection
        .invoke("AcceptVoiceCalling", this.voiceCallingModel)
        .catch((err: any) => {
          console.error(err);
        });

      try {
        const stream = await (navigator.mediaDevices as any).getUserMedia({
          video: false,
          audio: true,
        });
        if (!stream)
          throw new Error("Failed to get stream. is SSL configured?");

        this.streamVoiceCalling = stream;
        if (
          this.connection.myVoiceCallingPeer &&
          !this.connection.myVoiceCallingPeer.destroyed
        ) {
          console.log("add stream");
          this.connection.myVoiceCallingPeer.addStream(stream);
        }
      } catch (err) {
        console.error(err);
      }
    },
    async onVoiceCallingAccepted(data: any) {
      console.log("onVoiceCallingAccepted");

      this.accepted = true;
      this.showVoiceCalling = true;
      this.waitingSignal.pause();
      this.connection.otherVoiceCallingPeers = [data.to];
      if (
        this.connection.myVoiceCallingPeer &&
        !this.connection.myVoiceCallingPeer.destroyed
      ) {
        this.connection.myVoiceCallingPeer.destroy();
      }

      this.connection.myVoiceCallingPeer =
        await this.initializeVoiceCallingPeer(true);

      try {
        const stream = await (navigator.mediaDevices as any).getUserMedia({
          video: false,
          audio: true,
        });
        if (!stream)
          throw new Error("Failed to get stream. is SSL configured?");

        this.streamVoiceCalling = stream;
        if (
          this.connection.myVoiceCallingPeer &&
          !this.connection.myVoiceCallingPeer.destroyed
        ) {
          console.log("add stream");
          this.connection.myVoiceCallingPeer.addStream(stream);
        }
      } catch (err) {
        console.error(err);
      }
    },
    rejectIncomingCall() {
      console.log("rejectIncomingCall");
      this.connection
        .invoke("RejectVoiceCalling", this.voiceCallingModel)
        .catch((err: any) => {
          console.error(err);
        });
      this.isCalling = false;
      this.accepted = false;
      this.showVoiceCalling = false;
      this.incomingCallAudio.pause();
    },
    async onVoiceCallingRejected(data: any) {
      console.log("onVoiceCallingRejected");
      this.voiceCallingModel = data;
      this.isCalling = false;
      this.accepted = false;
      this.isOutcomingCall = false;
      this.showVoiceCalling = false;
      this.waitingSignal.pause();
      if (this.isCaller) this.busySignal.play();
    },
    stopCalling() {
      console.log("stopCalling");
      this.connection
        .invoke("StopVoiceCalling", this.voiceCallingModel)
        .catch((err: any) => {
          console.error(err);
        });
      this.isCalling = false;
      this.accepted = false;
      this.showVoiceCalling = false;
    },
    async onVoiceCallingStoped(data: any) {
      console.log("onVoiceCallingStoped");
      this.voiceCallingModel = data;
      this.isCalling = false;
      this.accepted = false;
      this.showVoiceCalling = false;
      // console.log(this.streamVoiceCalling);
      if (this.streamVoiceCalling) {
        this.streamVoiceCalling.getTracks().forEach(function (track: any) {
          track.stop();
        });
      }
      if (
        this.connection.myVoiceCallingPeer &&
        !this.connection.myVoiceCallingPeer.destroyed
      )
        this.connection.myVoiceCallingPeer.destroy();
    },
    voiceCall(account: any, autoAccept: any = false) {
      console.log("voice call account", account);
      // console.log("autoAccept", autoAccept);
      // console.log("voiceCall");
      this.voiceCallingModel = {
        from: this.connection.identifier,
        to: account,
        autoAcceptCall: autoAccept,
      };
      if (!autoAccept) this.waitingSignal.play();
      this.isCaller = true;
      this.isCalling = true;
      this.isOutcomingCall = true;
      this.accepted = false;
      this.showVoiceCalling = true;
      this.connection
        .invoke("StartVoiceCall", this.voiceCallingModel)
        .catch((err: any) => {
          console.error(err);
        });
    },
    async onVoiceCalling(data: any) {
      console.log("onVoiceCalling");
      this.voiceCallingModel = Object.assign({}, data);
      this.isCalling = true;
      this.accepted = false;
      this.isOutcomingCall = false;
      this.showVoiceCalling = true;

      if (this.voiceCallingModel.autoAcceptCall)
        await this.acceptIncomingCall();
      else this.incomingCallAudio.play();
    },
    async showMonitor(user: HocVienDTO) {
      this.showScreenSharing = true;
      this.userScreenSelected = Object.assign({}, user);
      this.connection
        .invoke("WatchScreenShare", user.account)
        .catch((err: any) => {
          console.error(err);
        });
      this.connection.otherScreenSharingPeers = [user.account];
      if (
        this.connection.myScreenSharingPeer &&
        !this.connection.myScreenSharingPeer.destroyed
      ) {
        this.connection.myScreenSharingPeer.destroy();
        this.connection.otherScreenSharingPeers.forEach((user: any) => {
          this.connection
            .invoke("RequestDisconnection", user)
            .catch((err: any) => {
              console.error(err);
            });
        });
      } else {
        this.connection.myScreenSharingPeer = await this.initializePeer(true);
        (this.$refs.btnCall as any).click();
      }
    },
    onRequestDisconnection(data: any) {
      //data is user request watch my share screen
      this.destroyMyPeer();
      this.connection.otherScreenSharingPeers.forEach((user: any) => {
        this.connection
          .invoke("RequestOtherDisconnection", user)
          .catch((err: any) => {
            console.error(err);
          });
      });
      this.connection
        .invoke("ResponseDisconnection", data)
        .catch((err: any) => {
          console.error(err);
        });
    },
    async onResponseDisconnection() {
      this.connection.myScreenSharingPeer = await this.initializePeer(true);
      (this.$refs.btnCall as any).click();
    },
    onRequestOtherDisconnection() {
      this.destroyMyPeer();
    },
    async onStartScreenMonitoring(data: any) {
      switch (data.action) {
        case "signal":
          // console.log(
          //   "onStartScreenMonitoring",
          //   data.signalData.from,
          //   data.signalData.to
          // );

          // if (
          //   this.connection.otherScreenSharingPeers.indexOf(
          //     data.signalData.from
          //   ) == -1
          // )
          // this.connection.otherScreenSharingPeers.push(dalta.signalData.from);
          this.connection.otherScreenSharingPeers = [data.signalData.from];
          if (
            this.connection.myScreenSharingPeer &&
            !this.connection.myScreenSharingPeer.destroyed
          ) {
            // this.connection.myScreenSharingPeer =
            //   this.connection.myScreenSharingPeer;
          } else {
            this.connection.myScreenSharingPeer = await this.initializePeer(
              false
            );
          }
          this.connection.myScreenSharingPeer.signal(data.signalData);
          break;
        case "voiceCallingSignal":
          // if (
          //   this.connection.otherVoiceCallingPeers.indexOf(
          //     data.signalData.from
          //   ) == -1
          // )
          this.connection.otherVoiceCallingPeers = [data.signalData.from];
          if (
            this.connection.myVoiceCallingPeer &&
            !this.connection.myVoiceCallingPeer.destroyed
          ) {
            // this.connection.myVoiceCallingPeer =
            //   this.connection.myVoiceCallingPeer;
          } else {
            this.connection.myVoiceCallingPeer =
              await this.initializeVoiceCallingPeer(false);
          }
          // console.log(this.connection.myVoiceCallingPeer);
          this.connection.myVoiceCallingPeer.signal(data.signalData);
          break;
        default:
          throw new Error("Hub method not implemented!" + data.action);
      }
    },
    async startScreenshare() {
      try {
        const stream = await (navigator.mediaDevices as any).getDisplayMedia({
          video: {
            height: 1080,
            width: 1920,
            aspectRatio: 1.7777777777,
            frameRate: 240,
          },
          // video: true,
          audio: true,
        });
        if (!stream)
          throw new Error("Failed to get stream. is SSL configured?");
        this.updateScreenShare(true);
        let userShareInfo = {
          id: this.myself.id,
          hoTen: this.myself.name,
          linkAnhCaNhan: this.myself.profilePicture,
          account: this.myself.account,
        } as HocVienDTO;
        this.connection
          .invoke("StartShareScreen", userShareInfo)
          .catch((err: any) => {
            console.error(err);
          });
        let videoTrack = stream.getVideoTracks()[0];
        videoTrack.onended = () => {
          this.updateScreenShare(false);
          console.log("stream ended");
          this.connection
            .invoke("StopShareScreen", userShareInfo)
            .catch((err: any) => {
              console.error(err);
            });
          this.stopCalling();
        };
        this.stream = stream;
        console.log("startScreenshare ~ this.stream", this.stream);
        if (this.connection.myScreenSharingPeer) {
          console.log(
            "Before ~ this.connection.myScreenSharingPeer",
            this.connection.myScreenSharingPeer
          );
          this.connection.myScreenSharingPeer.addStream(stream);
          console.log(
            "After ~ this.connection.myScreenSharingPeer",
            this.connection.myScreenSharingPeer
          );
        }
      } catch (err) {
        console.log(err);
      }
    },
    async initializePeer(initiator = false) {
      const peer = new Peer({
        initiator: initiator,
        trickle: false,
        stream: this.stream,
      });
      // peer._debug = console.log;
      peer.on("signal", (data: any) => {
        // console.log("~ peer.on ~ signal", data);
        // console.log("on signal", data.from, data.to);
        // let signalFrom = data.from;
        data.from = this.connection.identifier;
        for (
          let i = 0;
          i < this.connection.otherScreenSharingPeers.length;
          i++
        ) {
          data.to = this.connection.otherScreenSharingPeers[i];
          // console.log("signal to " + data.to, data);
          this.connection
            .invoke("SignalToUser", {
              action: "signal",
              signalData: data,
            })
            .catch((err: any) => {
              console.error(err);
            });
        }
      });
      peer.on("connect", () => {
        console.log("Peer screen is connected");
        this.connectFailed = false;
      });
      peer.on("data", (data: any) => {
        console.log("data" + data);
      });
      peer.on("close", () => {
        console.log("Peer screen is closed");
        if (this.isCalling) {
          this.stopCalling();
        }
      });
      peer.on("error", (err: any) => {
        this.connectFailed = true;
        console.log("Peer screen is error");
      });
      peer.on("stream", (stream: any) => {
        if (!this.showScreenSharing) return;
        console.log("Screen stream started");
        const videoWrapper =
          document.getElementById("video-wrapper") ||
          document.createElement("div");
        videoWrapper.setAttribute("id", "video-wrapper");
        const videoEl =
          document.querySelector("video") || document.createElement("video");
        console.log("peer.on ~ stream", stream);
        if ("srcObject" in videoEl) {
          videoEl.srcObject = stream;
        } else {
          (videoEl as any).src = window.URL.createObjectURL(stream); // for older browsers
        }
        // videoEl.controls = true;
        videoEl.autoplay = true;
        videoWrapper.append(videoEl);
        const connectionWrapper = (
          document.getElementById("connect-wrapper") as any
        ).parentNode;
        (connectionWrapper as any).hidden = true;
        (connectionWrapper as any).parentNode.append(videoWrapper);
        videoEl.play();
      });
      return peer;
    },
    async initializeVoiceCallingPeer(initiator = false) {
      const peer = new Peer({
        initiator: initiator,
        trickle: false,
        stream: this.streamVoiceCalling,
      });
      // peer._debug = console.log;
      peer.on("signal", (data: any) => {
        // console.log("on signal", data.from, data.to);
        // let signalFrom = data.from;
        data.from = this.connection.identifier;
        for (
          let i = 0;
          i < this.connection.otherVoiceCallingPeers.length;
          i++
        ) {
          data.to = this.connection.otherVoiceCallingPeers[i];
          // console.log("signal to " + data.to, data);
          this.connection
            .invoke("SignalToUser", {
              action: "voiceCallingSignal",
              signalData: data,
            })
            .catch((err: any) => {
              console.error(err);
            });
        }
      });
      peer.on("connect", () => {
        console.log("Peer voice is connected");
        this.connectFailed = false;
      });
      peer.on("data", (data: any) => {
        console.log("data" + data);
      });
      peer.on("close", () => {
        console.log("Peer voice is closed");
      });
      peer.on("error", (err: any) => {
        console.log("Peer voice error", err);
        this.connectFailed = true;
      });
      peer.on("stream", (stream: any) => {
        console.log("stream voice started");
        this.incomingCallAudio.pause();
        this.remoteAudio.srcObject = stream;
        this.remoteAudio.play();
      });
      return peer;
    },
    destroyMyPeer() {
      if (
        this.connection.myScreenSharingPeer &&
        !this.connection.myScreenSharingPeer.destroyed
      ) {
        this.connection.myScreenSharingPeer.destroy();
      }
      if (
        this.connection.myVoiceCallingPeer &&
        !this.connection.myVoiceCallingPeer.destroyed
      ) {
        this.connection.myVoiceCallingPeer.destroy();
      }
    },
  },
});
</script>

<style>
.chat-container {
  position: fixed;
  box-shadow: 0px 0px 25px 0px rgba(45, 69, 95, 0.1);
  transition: all 0.5s ease;
}
.chat-content {
  top: 45px;
  overflow: auto;
}
.chat-container .v-list {
  padding: 0;
}
.chat-container
  .v-text-field.v-text-field--enclosed:not(.v-text-field--rounded)
  > .v-input__control
  > .v-input__slot {
  padding: 0 8px;
}
.chat-group-title {
  /* padding: 8px 0px 8px 10px; */
  color: #4a4a4a;
  font-size: 14px;
  font-weight: bold;
}
.chat-item-user {
  padding: 0 !important;
}
.chat-username {
  margin-left: 6px;
  font-size: 16px;
  color: #3f414d;
  font-weight: 400;
}
.lastMessage {
  font-size: 14px;
  margin-left: 6px;
}
.border-avatar {
  border-radius: inherit;
}
.chat-btn-collapse {
  position: fixed;
  bottom: 20px;
  right: 21.5px;
  transition: all 0.5s ease;
}
.chat-btn {
  position: fixed;
  bottom: 20px;
  right: 250px;
  transition: all 0.5s ease;
}
.chat-btn .v-btn--fab.v-size--default {
  height: 46px;
  width: 46px;
}
.chat-btn-collapse .v-btn--fab.v-size--default {
  height: 46px;
  width: 46px;
}
.avatar-badge .v-badge__badge {
  left: calc(100% - 12px) !important;
  top: calc(100% - 12px) !important;
}
.border-avatar .v-badge--dot .v-badge__badge {
  height: 12px !important;
  width: 12px !important;
  border-radius: 50% !important;
}
.chat-drawer-menu--scroll {
  height: calc(100vh - 50px);
  overflow: auto;
}
/* .btn-show-chat {
  background-image: linear-gradient(#34b9e5, #0089dc);
}
.box-btn-show-chat {
  position: fixed;
  bottom: 8.5px;
  right: 9.5px;
  width: 70px;
  height: 70px;
  display: flex;
  justify-content: center;
  align-items: center;
}
.box-btn-show-chat .circle-1 {
  width: 100%;
  height: 100%;
  border: 1px solid rgb(68, 68, 250);
  position: absolute;
  border-radius: 50%;
  animation: glow-circle 2s infinite linear;
  -moz-animation: glow-circle 2s infinite linear;
  -webkit-animation: glow-circle 2s infinite linear;
  -o-animation: glow-circle 2s infinite linear;
}
.box-btn-show-chat .circle-1:nth-child(1) {
  animation-delay: 0.8s;
}
@keyframes glow-circle {
  0% {
    opacity: 0;
    transform: scale(0);
  }
  70% {
    opacity: 1;
    transform: scale(0.8);
  }
  100% {
    opacity: 0;
    transform: scale(1.1);
  }
} */
.button-wrapper {
  position: fixed;
  top: 0;
  right: 0;
  margin: 16px;
  z-index: 2;
}
#connect-wrapper {
  width: 100%;
  height: 100%;
}
.connect-error {
  font-size: 18px;
  color: #fff;
}
#video-wrapper {
  position: absolute;
  top: 0;
  bottom: 0;
  width: 100%;
  height: 100%;
}
#video-wrapper video {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 100%;
  height: 100%;
}

.user-share-screen-info__wrapper {
  position: fixed;
  top: 0;
  width: 100vw;
  display: flex;
  justify-content: center;
  z-index: 1;
}
.user-share-screen-info__content {
  color: #fff;
  padding: 4px 12px;
  background: #4caf50;
  border-radius: 0 0 4px 4px;
}
</style>
